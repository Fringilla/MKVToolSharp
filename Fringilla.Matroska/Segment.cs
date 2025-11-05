namespace Fringilla.Matroska;

[EbmlElement(Matroska.ElementId.Segment, "Segment")]
public sealed class Segment : MatroskaElement
{
    public override string ElementName => "Segment";
    public override ulong ElementId => Matroska.ElementId.Segment;

    [EbmlElement(Matroska.ElementId.Info, "Info")]
    public SegmentInfo? Info { get; set; }

    [EbmlElement(Matroska.ElementId.Tracks, "Tracks")]
    public List<TrackEntry> Tracks { get; set; } = new();

    [EbmlElement(Matroska.ElementId.Cluster, "Cluster")]
    public List<Cluster> Clusters { get; set; } = new();

    [EbmlElement(Matroska.ElementId.SeekHead, "SeekHead")]
    public SeekHead? SeekHead { get; set; }

    [EbmlElement(Matroska.ElementId.Cues, "Cues")]
    public Cues? Cues { get; set; }

    [EbmlElement(Matroska.ElementId.Attachments, "Attachments")]
    public Attachments? Attachments { get; set; }

    [EbmlElement(Matroska.ElementId.Chapters, "Chapters")]
    public Chapters? Chapters { get; set; }

    [EbmlElement(Matroska.ElementId.Tags, "Tags")]
    public Tags? Tags { get; set; }
}