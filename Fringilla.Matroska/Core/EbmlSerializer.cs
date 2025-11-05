namespace Fringilla.Matroska;

using System.Reflection;
using System.Collections;
using System.Buffers.Binary;

/// <summary>
/// Serializes Matroska/EBML elements to a stream using reflection and [EbmlElement] attributes.
/// </summary>
public static class EbmlSerializer
{
    /// <summary>
    /// Serializes a Matroska element into the specified stream.
    /// </summary>
    public static void Serialize(Stream stream, object element)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream));
        if (element is null) throw new ArgumentNullException(nameof(element));

        WriteMasterElement(stream, element);
    }

    private static void WriteMasterElement(Stream stream, object element)
    {
        var elementType = element.GetType();
        var elementAttr = elementType.GetCustomAttribute<EbmlElementAttribute>()
            ?? throw new InvalidOperationException($"Type {elementType.FullName} is missing [EbmlElement].");

        using var temp = new MemoryStream();
        WriteObjectContent(temp, element);
        var contentBytes = temp.ToArray();

        EbmlBinary.WriteElementHeader(stream, elementAttr.Id, (ulong)contentBytes.Length);
        stream.Write(contentBytes, 0, contentBytes.Length);
    }

    private static void WriteObjectContent(Stream stream, object element)
    {
        var props = element.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(p => new
            {
                Property = p,
                Attribute = p.GetCustomAttribute<EbmlElementAttribute>()
            })
            .Where(x => x.Attribute != null)
            .OrderBy(x => x.Attribute!.Order)
            .ToList();

        foreach (var (property, attr) in props)
        {
            var val = property.GetValue(element);
            if (val is null)
                continue;

            switch (val)
            {
                // --- Primitive numeric types ---
                case int i32:
                    WriteInteger(stream, attr!, (ulong)i32);
                    break;
                case long i64:
                    WriteInteger(stream, attr!, (ulong)i64);
                    break;
                case uint u32:
                    WriteInteger(stream, attr!, u32);
                    break;
                case ulong u64:
                    WriteInteger(stream, attr!, u64);
                    break;
                case short i16:
                    WriteInteger(stream, attr!, (ulong)i16);
                    break;
                case byte b:
                    WriteInteger(stream, attr!, b);
                    break;

                // --- Floating point ---
                case float f:
                    WriteFloat(stream, attr!, f);
                    break;
                case double d:
                    WriteFloat(stream, attr!, d);
                    break;

                // --- Strings ---
                case string s when !string.IsNullOrEmpty(s):
                    WriteString(stream, attr!, s);
                    break;

                // --- Collections ---
                case IEnumerable enumerable and not string:
                    foreach (var child in enumerable.Cast<object>().Where(o => o != null))
                        WriteMasterElement(stream, child);
                    break;

                // --- Nested EBML element ---
                default:
                    if (property.PropertyType.GetCustomAttribute<EbmlElementAttribute>() != null)
                        WriteMasterElement(stream, val);
                    else
                        throw new NotSupportedException($"Unsupported property type: {property.PropertyType.FullName}");
                    break;
            }
        }
    }

    private static void WriteInteger(Stream stream, EbmlElementAttribute attr, ulong value)
    {
        var bytes = UlongToMinimalBytes(value);
        EbmlBinary.WriteElementHeader(stream, attr.Id, (ulong)bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
    }

    private static void WriteFloat(Stream stream, EbmlElementAttribute attr, double value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        EbmlBinary.WriteElementHeader(stream, attr.Id, (ulong)bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
    }

    private static void WriteString(Stream stream, EbmlElementAttribute attr, string value)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(value);
        EbmlBinary.WriteElementHeader(stream, attr.Id, (ulong)bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Converts a ulong to the minimal number of big-endian bytes.
    /// </summary>
    private static byte[] UlongToMinimalBytes(ulong v)
    {
        Span<byte> tmp = stackalloc byte[8];
        int len = 0;
        do
        {
            tmp[7 - len++] = (byte)(v & 0xFF);
            v >>= 8;
        } while (v != 0);
        return tmp[^len..].ToArray();
    }
}
