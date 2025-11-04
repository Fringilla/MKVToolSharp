namespace Fringilla.Matroska;

[EbmlElement(ElementId.Segment, "Segment")]
public sealed class Segment : MatroskaElement
{
    public override string Name => "Segment";
    public override ulong Id => ElementId.Segment;

    [EbmlElement(ElementId.Info, "Info")]
    public SegmentInfo? Info { get; set; }

    [EbmlElement(ElementId.Tracks, "Tracks")]
    public List<TrackEntry> Tracks { get; set; } = new();

    [EbmlElement(ElementId.Cluster, "Cluster")]
    public List<Cluster> Clusters { get; set; } = new();

    [EbmlElement(ElementId.SeekHead, "SeekHead")]
    public SeekHead? SeekHead { get; set; }

    [EbmlElement(ElementId.Cues, "Cues")]
    public Cues? Cues { get; set; }

    [EbmlElement(ElementId.Attachments, "Attachments")]
    public Attachments? Attachments { get; set; }

    [EbmlElement(ElementId.Chapters, "Chapters")]
    public Chapters? Chapters { get; set; }

    [EbmlElement(ElementId.Tags, "Tags")]
    public Tags? Tags { get; set; }
}