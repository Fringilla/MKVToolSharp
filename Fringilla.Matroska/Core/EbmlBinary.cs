namespace Fringilla.Matroska;

using System;
using System.IO;
using System.Buffers.Binary;

internal static class EbmlBinary
{
    // Encode VINT for size (returns bytes of VINT). Supports sizes up to 8 bytes.
    public static byte[] EncodeVInt(ulong value, int minLength = 1)
    {
        // choose minimal length where value fits into (7*len) bits for size
        for (int len = Math.Max(1, minLength); len <= 8; len++)
        {
            ulong max = (1UL << (7 * len)) - 1;
            if (value <= max)
            {
                var bytes = new byte[len];
                // leading marker: 1 << (8-len)
                bytes[0] = (byte)((1 << (8 - len)) | (int)((value >> (8 * (len - 1))) & (ulong)((1 << (8 - len)) - 1)));
                for (int i = 1; i < len; i++)
                    bytes[i] = (byte)((value >> (8 * (len - i - 1))) & 0xFF);
                return bytes;
            }
        }
        throw new ArgumentOutOfRangeException(nameof(value), "VInt value too large");
    }

    // Decode VInt (returns (value, lengthInBytes))
    public static (ulong value, int length) DecodeVInt(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length == 0) throw new ArgumentException("Buffer empty");
        byte first = buffer[0];
        int leadingOnes = 0;
        for (int i = 0; i < 8; i++)
        {
            if ((first & (1 << (7 - i))) != 0) { leadingOnes++; break; }
        }
        // EBML vints are encoded by counting leading zeros, but easier approach:
        int len = 1;
        // count leading zeros until first 1; position determines length
        int zeroBits = 0;
        for (int bit = 7; bit >= 0; bit--)
        {
            if ((first & (1 << bit)) == 0) zeroBits++;
            else break;
        }
        len = zeroBits + 1;
        if (len > buffer.Length) throw new ArgumentException("Buffer too small for VInt");
        // Clear length marker bits on first byte
        ulong value = (ulong)(first & ((1 << (8 - len)) - 1));
        for (int i = 1; i < len; i++)
            value = (value << 8) | buffer[i];
        return (value, len);
    }

    // Write EBML element header: element id (raw big-endian bytes) + size (vint)
    public static void WriteElementHeader(Stream s, ulong id, ulong contentSize)
    {
        // Element ID can be 1..4 bytes (we will write full 4-byte if >1)
        var idBytes = IdToBytes(id);
        s.Write(idBytes, 0, idBytes.Length);
        var sizeBytes = EncodeVInt(contentSize);
        s.Write(sizeBytes, 0, sizeBytes.Length);
    }

    public static byte[] IdToBytes(ulong id)
    {
        // we assume id fits in 1..4 bytes; choose minimal bytes representing id
        if (id <= 0xFF) return new byte[] { (byte)id };
        if (id <= 0xFFFF) return new byte[] { (byte)(id >> 8), (byte)id };
        if (id <= 0xFFFFFF) return new byte[] { (byte)(id >> 16), (byte)(id >> 8), (byte)id };
        if (id <= 0xFFFFFFFF) return new byte[] { (byte)(id >> 24), (byte)(id >> 16), (byte)(id >> 8), (byte)id };

        throw new ArgumentOutOfRangeException(nameof(id), $"Element ID {id:X} exceeds 4 bytes limit");
    }

    public static ulong BytesToUlong(ReadOnlySpan<byte> bytes)
    {
        ulong v = 0;
        foreach (var b in bytes) { v = (v << 8) | b; }
        return v;
    }
}
