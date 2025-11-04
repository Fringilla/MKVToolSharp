namespace Fringilla.Matroska;

[EbmlElement(0xBB, "CuePoint")]
public sealed class CuePoint : MatroskaElement
{
    public override string Name => "CuePoint";
    public override ulong Id => 0xBB;

    [EbmlElement(0xB3, "CueTime")]
    public ulong Time { get; set; }

    [EbmlElement(0xB7, "CueTrackPositions")]
    public List<CueTrackPosition> Positions { get; set; } = new();
}