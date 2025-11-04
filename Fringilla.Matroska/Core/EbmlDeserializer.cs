namespace Fringilla.Matroska;

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public sealed class EbmlDeserializer
{
    public T Deserialize<T>(Stream s) where T : MatroskaElement, new()
    {
        // read element header from stream (ID + size)
        var id = ReadElementId(s, out _);
        var size = ReadVIntAsUlong(s);
        var payload = ReadExactly(s, (long)size);

        var t = new T();
        var expectedAttr = typeof(T).GetCustomAttribute<EbmlElementAttribute>();
        if (expectedAttr == null) throw new InvalidOperationException("Type missing EbmlElement attribute");
        if (expectedAttr.Id != id)
        {
            // allow if top-level id mismatch; but continue to attempt decode
        }

        using var ms = new MemoryStream(payload);
        ReadIntoObject(ms, t, (long)size);
        return t;
    }

    private void ReadIntoObject(Stream s, object target, long lengthLimit)
    {
        var propsById = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(p => (prop: p, attr: p.GetCustomAttribute<EbmlElementAttribute>()))
            .Where(x => x.attr != null)
            .ToDictionary(x => x.attr!.Id, x => x.prop!);

        long startPos = s.Position;
        while (s.Position - startPos < lengthLimit)
        {
            var id = ReadElementId(s, out int idLen);
            var size = ReadVIntAsUlong(s);
            var content = ReadExactly(s, (long)size);

            if (!propsById.TryGetValue(id, out var prop))
            {
                // unknown element - skip
                continue;
            }

            var propType = prop.PropertyType;
            if (propType == typeof(string) || propType == typeof(string?))
            {
                var str = Encoding.UTF8.GetString(content);
                prop.SetValue(target, str);
                continue;
            }

            if (propType == typeof(byte[]))
            {
                prop.SetValue(target, content);
                continue;
            }

            if (propType == typeof(ulong) || propType == typeof(ulong?))
            {
                ulong v = EbmlBinary.BytesToUlong(content);
                prop.SetValue(target, v);
                continue;
            }

            if (typeof(IList).IsAssignableFrom(propType))
            {
                // assume list of master elements
                var itemType = propType.GetGenericArguments().FirstOrDefault();
                if (itemType == null) continue;
                using var ms = new MemoryStream(content);
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;
                while (ms.Position < ms.Length)
                {
                    var childId = ReadElementId(ms, out _);
                    var childSize = ReadVIntAsUlong(ms);
                    var childPayload = ReadExactly(ms, (long)childSize);
                    // construct instance
                    var child = Activator.CreateInstance(itemType)!;
                    using var cms = new MemoryStream(childPayload);
                    ReadIntoObject(cms, child, (long)childSize);
                    list.Add(child);
                }
                prop.SetValue(target, list);
                continue;
            }

            // nested master element
            if (propType.GetCustomAttribute<EbmlElementAttribute>() != null)
            {
                var nested = Activator.CreateInstance(propType)!;
                using var ms = new MemoryStream(content);
                ReadIntoObject(ms, nested, (long)size);
                prop.SetValue(target, nested);
                continue;
            }

            // fallback for ints
            if (propType == typeof(int) || propType == typeof(int?))
            {
                var v = (int)EbmlBinary.BytesToUlong(content);
                prop.SetValue(target, v);
                continue;
            }

            // other types: skip for now
        }
    }

    private static byte[] ReadExactly(Stream s, long count)
    {
        var buf = new byte[count];
        var read = 0;
        while (read < count)
        {
            var r = s.Read(buf, read, (int)(count - read));
            if (r == 0) throw new EndOfStreamException();
            read += r;
        }
        return buf;
    }

    private static ulong ReadVIntAsUlong(Stream s)
    {
        // Read first byte to determine length
        int b = s.ReadByte();
        if (b < 0) throw new EndOfStreamException();
        byte first = (byte)b;
        int zeroBits = 0;
        for (int bit = 7; bit >= 0; bit--)
        {
            if ((first & (1 << bit)) == 0) zeroBits++;
            else break;
        }
        int len = zeroBits + 1;
        var buffer = new byte[len];
        buffer[0] = first;
        if (len - 1 > 0)
        {
            var read = s.Read(buffer, 1, len - 1);
            if (read != len - 1) throw new EndOfStreamException();
        }
        var (value, _) = EbmlBinary.DecodeVInt(buffer);
        return value;
    }

    private static ulong ReadElementId(Stream s, out int length)
    {
        // read up to 4 bytes to capture id
        var tmp = new byte[4];
        int r = s.Read(tmp, 0, 1);
        if (r != 1) throw new EndOfStreamException();
        // Determine id length by counting leading zero bits in first byte
        byte first = tmp[0];
        int zeroBits = 0;
        for (int bit = 7; bit >= 0; bit--)
        {
            if ((first & (1 << bit)) == 0) zeroBits++;
            else break;
        }
        int len = zeroBits + 1;
        tmp[0] = first;
        if (len - 1 > 0)
        {
            var rr = s.Read(tmp, 1, len - 1);
            if (rr != len - 1) throw new EndOfStreamException();
        }
        length = len;
        return EbmlBinary.BytesToUlong(tmp.AsSpan(0, len));
    }
}
