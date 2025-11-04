namespace Fringilla.Matroska;

[EbmlElement(0x45B9, "EditionEntry")]
public sealed class ChapterEdition : MatroskaElement
{
    public override string Name => "EditionEntry";
    public override ulong Id => 0x45B9;

    [EbmlElement(0xB6, "ChapterAtom")]
    public List<ChapterAtom> Chapters { get; set; } = new();
}