namespace Fringilla.Matroska;

[EbmlElement(0x80, "ChapterDisplay")]
public sealed class ChapterDisplay : MatroskaElement
{
    public override string Name => "ChapterDisplay";
    public override ulong Id => 0x80;

    [EbmlElement(0x85, "ChapString")]
    public string Text { get; set; } = string.Empty;

    [EbmlElement(0x437C, "ChapLanguage")]
    public string? Language { get; set; }
}