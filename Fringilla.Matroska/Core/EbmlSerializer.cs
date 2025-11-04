namespace Fringilla.Matroska;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public sealed class EbmlSerializer
{
    public void Serialize<T>(Stream s, T element) where T : MatroskaElement
    {
        var type = element!.GetType();
        var attr = type.GetCustomAttribute<EbmlElementAttribute>();
        if (attr == null) throw new InvalidOperationException("Type missing EbmlElement attribute");

        using var ms = new MemoryStream();
        // write content into ms first
        WriteObjectContent(ms, element);
        // element header
        EbmlBinary.WriteElementHeader(s, attr.Id, (ulong)ms.Length);
        ms.Position = 0;
        ms.CopyTo(s);
    }

    private void WriteObjectContent(Stream s, object obj)
    {
        var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .Where(p => p.GetCustomAttribute<EbmlElementAttribute>() != null)
                     .OrderBy(p => p.MetadataToken); // stable order, not spec-order but ok for PoC

        foreach (var p in props)
        {
            var pAttr = p.GetCustomAttribute<EbmlElementAttribute>()!;
            var val = p.GetValue(obj);
            if (val == null) continue;

            if (val is string str)
            {
                var bytes = Encoding.UTF8.GetBytes(str);
                EbmlBinary.WriteElementHeader(s, pAttr.Id, (ulong)bytes.Length);
                s.Write(bytes, 0, bytes.Length);
                continue;
            }

            if (val is ulong u64)
            {
                // write as big-endian integer with minimal bytes
                var bytes = UlongToMinimalBytes(u64);
                EbmlBinary.WriteElementHeader(s, pAttr.Id, (ulong)bytes.Length);
                s.Write(bytes, 0, bytes.Length);
                continue;
            }

            if (val is int i32)
            {
                var bytes = UlongToMinimalBytes((ulong)i32);
                EbmlBinary.WriteElementHeader(s, pAttr.Id, (ulong)bytes.Length);
                s.Write(bytes, 0, bytes.Length);
                continue;
            }

            if (val is double d)
            {
                var buf = BitConverter.GetBytes(d);
                if (BitConverter.IsLittleEndian) Array.Reverse(buf);
                EbmlBinary.WriteElementHeader(s, pAttr.Id, (ulong)buf.Length);
                s.Write(buf, 0, buf.Length);
                continue;
            }

            if (val is byte[] bin)
            {
                EbmlBinary.WriteElementHeader(s, pAttr.Id, (ulong)bin.Length);
                s.Write(bin, 0, bin.Length);
                continue;
            }

            // IEnumerable -> list of master elements
            if (val is IEnumerable ie && !(val is string))
            {
                foreach (var child in ie)
                {
                    WriteMasterElement(s, child);
                }
                continue;
            }

            // single nested master element
            if (HasEbmlAttr(val!.GetType()))
            {
                WriteMasterElement(s, val!);
                continue;
            }

            // fallback: try to handle as uinteger if numeric
            var tCode = Type.GetTypeCode(val!.GetType());
            if (tCode == TypeCode.UInt64 || tCode == TypeCode.UInt32 || tCode == TypeCode.Int32)
            {
                var bytes = UlongToMinimalBytes(Convert.ToUInt64(val));
                EbmlBinary.WriteElementHeader(s, pAttr.Id, (ulong)bytes.Length);
                s.Write(bytes, 0, bytes.Length);
                continue;
            }

            throw new NotSupportedException($"Property type {p.PropertyType} not supported");
        }
    }

    private static bool HasEbmlAttr(Type t) => t.GetCustomAttribute<EbmlElementAttribute>() != null;

    private void WriteMasterElement(Stream s, object child)
    {
        var t = child.GetType();
        var a = t.GetCustomAttribute<EbmlElementAttribute>();
        if (a == null) throw new InvalidOperationException("Child missing EbmlElement attribute");

        using var ms = new MemoryStream();
        WriteObjectContent(ms, child);
        EbmlBinary.WriteElementHeader(s, a.Id, (ulong)ms.Length);
        ms.Position = 0;
        ms.CopyTo(s);
    }

    private static byte[] UlongToMinimalBytes(ulong v)
    {
        if (v == 0) return new byte[] { 0x00 };
        var tmp = new List<byte>();
        while (v != 0)
        {
            tmp.Add((byte)(v & 0xFF));
            v >>= 8;
        }
        tmp.Reverse();
        return tmp.ToArray();
    }
}
