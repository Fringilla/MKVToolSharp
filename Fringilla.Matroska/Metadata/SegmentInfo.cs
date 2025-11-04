namespace Fringilla.Matroska;

[EbmlElement(0x1549A966, "Info")]
public sealed class SegmentInfo : MatroskaElement
{
    public override string Name => "Info";
    public override ulong Id => 0x1549A966;

    [EbmlElement(0x2AD7B1, "TimecodeScale")]
    public ulong TimecodeScale { get; set; } = 1000000;

    [EbmlElement(0x4489, "Duration")]
    public double? Duration { get; set; }

    [EbmlElement(0x7BA9, "Title")]
    public string? Title { get; set; }

    [EbmlElement(0x4D80, "MuxingApp")]
    public string? MuxingApp { get; set; }

    [EbmlElement(0x5741, "WritingApp")]
    public string? WritingApp { get; set; }
}