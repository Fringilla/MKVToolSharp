namespace Fringilla.Matroska;

[EbmlElement(0xB7, "CueTrackPositions")]
public sealed class CueTrackPosition : MatroskaElement
{
    public override string ElementName => "CueTrackPositions";
    public override ulong ElementId => 0xB7;

    [EbmlElement(0xF7, "CueTrack")]
    public ulong Track { get; set; }

    [EbmlElement(0xF1, "CueClusterPosition")]
    public ulong ClusterPosition { get; set; }
}