namespace Fringilla.Matroska;

[EbmlElement(0x1043A770, "Chapters")]
public sealed class Chapters : MatroskaElement
{
    public override string Name => "Chapters";
    public override ulong Id => 0x1043A770;

    [EbmlElement(0x45B9, "EditionEntry")]
    public List<ChapterEdition> Editions { get; set; } = new();
}