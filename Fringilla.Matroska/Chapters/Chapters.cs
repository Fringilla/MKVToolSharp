namespace Fringilla.Matroska;

[EbmlElement(0x1043A770, "Chapters")]
public sealed class Chapters : MatroskaElement
{
    public override string ElementName => "Chapters";
    public override ulong ElementId => 0x1043A770;

    [EbmlElement(0x45B9, "EditionEntry")]
    public List<ChapterEdition> Editions { get; set; } = new();
}