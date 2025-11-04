namespace Fringilla.Matroska;

[EbmlElement(0xE0, "Video")]
public sealed class VideoTrack : MatroskaElement
{
    public override string Name => "Video";
    public override ulong Id => 0xE0;

    [EbmlElement(0xB0, "PixelWidth")]
    public int Width { get; set; }

    [EbmlElement(0xBA, "PixelHeight")]
    public int Height { get; set; }
}