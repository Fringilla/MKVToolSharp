namespace Fringilla.Matroska;

[EbmlElement(0x1941A469, "Attachments")]
public sealed class Attachments : MatroskaElement
{
    public override string Name => "Attachments";
    public override ulong Id => 0x1941A469;

    [EbmlElement(0x61A7, "AttachedFile")]
    public List<AttachedFile> Files { get; set; } = new();
}