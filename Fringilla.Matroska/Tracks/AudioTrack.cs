namespace Fringilla.Matroska;

[EbmlElement(0xE1, "Audio")]
public sealed class AudioTrack : MatroskaElement
{
    public override string ElementName => "Audio";
    public override ulong ElementId => 0xE1;

    [EbmlElement(0xB5, "SamplingFrequency")]
    public double SamplingFrequency { get; set; }

    [EbmlElement(0x9F, "Channels")]
    public int Channels { get; set; }
}