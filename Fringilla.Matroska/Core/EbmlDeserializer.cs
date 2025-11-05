namespace Fringilla.Matroska;

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public sealed class EbmlDeserializer
{
    public T Deserialize<T>(Stream s) where T : MatroskaElement, new()
    {
        var id = ReadElementId(s, out _);
        var size = ReadVIntAsUlong(s);
        var payload = ReadExactly(s, (long)size);

        var attr = typeof(T).GetCustomAttribute<EbmlElementAttribute>()
            ?? throw new InvalidOperationException($"Type {typeof(T).FullName} missing [EbmlElement]");
        
        // ID mismatch → waarschuwing, maar doorgaan
        if (attr.Id != id) { /* tolerant bij top-level elements */ }

        using var ms = new MemoryStream(payload);
        var t = new T();
        ReadIntoObject(ms, t, (long)size);
        return t;
    }

    private void ReadIntoObject(Stream s, object target, long lengthLimit)
    {
        var propsById = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(p => (prop: p, attr: p.GetCustomAttribute<EbmlElementAttribute>()))
            .Where(x => x.attr != null)
            .ToDictionary(x => x.attr!.Id, x => x.prop);

        var startPos = s.Position;
        while (s.Position - startPos < lengthLimit)
        {
            var id = ReadElementId(s, out _);
            var size = ReadVIntAsUlong(s);
            var content = ReadExactly(s, (long)size);

            if (!propsById.TryGetValue(id, out var prop))
                continue; // onbekend element overslaan

            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            // --- Primitive decodes ---
            object? value = DecodePrimitive(propType, content);
            if (value is not null)
            {
                prop.SetValue(target, value);
                continue;
            }

            // --- List / array ---
            if (typeof(IList).IsAssignableFrom(propType))
            {
                var itemType = propType.GetGenericArguments().FirstOrDefault();
                if (itemType == null)
                    continue;

                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;
                using var ms = new MemoryStream(content);
                while (ms.Position < ms.Length)
                    list.Add(DeserializeElement(ms, itemType));
                
                prop.SetValue(target, list);
                continue;
            }

            // --- Nested master element ---
            if (propType.GetCustomAttribute<EbmlElementAttribute>() != null)
            {
                using var ms = new MemoryStream(content);
                var nested = DeserializeElement(ms, propType);
                prop.SetValue(target, nested);
            }
        }
    }

    private static object? DecodePrimitive(Type propType, byte[] content)
    {
        if (propType == typeof(string))
            return Encoding.UTF8.GetString(content);
        if (propType == typeof(byte[]))
            return content;
        if (propType == typeof(ulong))
            return EbmlBinary.BytesToUlong(content);
        if (propType == typeof(long))
            return (long)EbmlBinary.BytesToUlong(content);
        if (propType == typeof(uint))
            return (uint)EbmlBinary.BytesToUlong(content);
        if (propType == typeof(int))
            return (int)EbmlBinary.BytesToUlong(content);
        if (propType == typeof(short))
            return (short)EbmlBinary.BytesToUlong(content);
        if (propType == typeof(byte))
            return content.Length > 0 ? content[^1] : (byte)0;
        if (propType == typeof(float))
        {
            var b = content.ToArray();
            if (BitConverter.IsLittleEndian) Array.Reverse(b);
            return BitConverter.ToSingle(b);
        }
        if (propType == typeof(double))
        {
            var b = content.ToArray();
            if (BitConverter.IsLittleEndian) Array.Reverse(b);
            return BitConverter.ToDouble(b);
        }

        return null;
    }

    private static object DeserializeElement(Stream s, Type t)
    {
        var id = ReadElementId(s, out _);
        var size = ReadVIntAsUlong(s);
        var payload = ReadExactly(s, (long)size);
        var element = Activator.CreateInstance(t)!;

        using var ms = new MemoryStream(payload);
        var method = typeof(EbmlDeserializer).GetMethod(nameof(ReadIntoObject), BindingFlags.NonPublic | BindingFlags.Instance)!;
        var deserializer = new EbmlDeserializer();
        method.Invoke(deserializer, new object[] { ms, element, (long)size });
        return element;
    }

    // --- Stream helpers ---

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
        if (len > 1 && s.Read(buffer, 1, len - 1) != len - 1)
            throw new EndOfStreamException();
        var (value, _) = EbmlBinary.DecodeVInt(buffer);
        return value;
    }

    private static ulong ReadElementId(Stream s, out int length)
    {
        var tmp = new byte[4];
        if (s.Read(tmp, 0, 1) != 1)
            throw new EndOfStreamException();

        byte first = tmp[0];
        int zeroBits = 0;
        for (int bit = 7; bit >= 0; bit--)
        {
            if ((first & (1 << bit)) == 0) zeroBits++;
            else break;
        }
        int len = zeroBits + 1;
        if (len > 1 && s.Read(tmp, 1, len - 1) != len - 1)
            throw new EndOfStreamException();
        length = len;
        return EbmlBinary.BytesToUlong(tmp.AsSpan(0, len));
    }
}
