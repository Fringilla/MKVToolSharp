namespace Fringilla.Matroska;

[EbmlElement(0x80, "ChapterDisplay")]
public sealed class ChapterDisplay : MatroskaElement
{
    public override string ElementName => "ChapterDisplay";
    public override ulong ElementId => 0x80;

    [EbmlElement(0x85, "ChapString")]
    public string Text { get; set; } = string.Empty;

    [EbmlElement(0x437C, "ChapLanguage")]
    public string? Language { get; set; }
}