namespace Fringilla.Matroska;

[EbmlElement(0x114D9B74, "SeekHead")]
public sealed class SeekHead : MatroskaElement
{
    public override string ElementName => "SeekHead";
    public override ulong ElementId => 0x114D9B74;

    [EbmlElement(0x4DBB, "Seek")]
    public List<SeekEntry> Entries { get; set; } = new();
}