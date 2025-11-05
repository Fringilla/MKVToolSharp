namespace Fringilla.Matroska;

[EbmlElement(0x1C53BB6B, "Cues")]
public sealed class Cues : MatroskaElement
{
    public override string ElementName => "Cues";
    public override ulong ElementId => 0x1C53BB6B;

    [EbmlElement(0xBB, "CuePoint")]
    public List<CuePoint> Points { get; set; } = new();
}