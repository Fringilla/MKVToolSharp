namespace Fringilla.Matroska;

[EbmlElement(0xA3, "SimpleBlock")]
public sealed class Block : MatroskaElement
{
    public override string Name => "SimpleBlock";
    public override ulong Id => 0xA3;

    public int TrackNumber { get; set; }
    public short Timecode { get; set; }
    public bool Keyframe { get; set; }
    public byte[] Data { get; set; } = [];
}