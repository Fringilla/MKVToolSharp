namespace Fringilla.Matroska;

[EbmlElement(0xAE, "TrackEntry")]
public sealed class TrackEntry : MatroskaElement
{
    public override string ElementName => "TrackEntry";
    public override ulong ElementId => 0xAE;

    [EbmlElement(0xD7, "TrackNumber")]
    public int TrackNumber { get; set; }

    [EbmlElement(Matroska.ElementId.TrackName, "TrackName")]
    public string? TrackName { get; set; }
    
    [EbmlElement(0x83, "TrackType")]
    public byte TrackType { get; set; } // 1=video, 2=audio, etc.

    [EbmlElement(0x86, "CodecID")]
    public string CodecId { get; set; } = string.Empty;

    [EbmlElement(0x258688, "CodecName")]
    public string? CodecName { get; set; }

    [EbmlElement(0xE0, "Video")]
    public VideoTrack? Video { get; set; }

    [EbmlElement(0xE1, "Audio")]
    public AudioTrack? Audio { get; set; }
}