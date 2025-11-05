namespace Fringilla.Matroska;

[EbmlElement(0x4DBB, "Seek")]
public sealed class SeekEntry : MatroskaElement
{
    public override string ElementName => "Seek";
    public override ulong ElementId => 0x4DBB;

    [EbmlElement(0x53AB, "SeekID")]
    public byte[]? SeekId { get; set; }

    [EbmlElement(0x53AC, "SeekPosition")]
    public ulong SeekPosition { get; set; }
}