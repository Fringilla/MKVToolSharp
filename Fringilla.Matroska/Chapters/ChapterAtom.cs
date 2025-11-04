namespace Fringilla.Matroska;

[EbmlElement(0xB6, "ChapterAtom")]
public sealed class ChapterAtom : MatroskaElement
{
    public override string Name => "ChapterAtom";
    public override ulong Id => 0xB6;

    [EbmlElement(0x73C4, "ChapterUID")]
    public ulong Uid { get; set; }

    [EbmlElement(0x91, "ChapterTimeStart")]
    public ulong TimeStart { get; set; }

    [EbmlElement(0x92, "ChapterTimeEnd")]
    public ulong? TimeEnd { get; set; }

    [EbmlElement(0x80, "ChapterDisplay")]
    public List<ChapterDisplay> Displays { get; set; } = new();
}