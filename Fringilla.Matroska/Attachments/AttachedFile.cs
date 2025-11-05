namespace Fringilla.Matroska;

[EbmlElement(0x61A7, "AttachedFile")]
public sealed class AttachedFile : MatroskaElement
{
    public override string ElementName => "AttachedFile";
    public override ulong ElementId => 0x61A7;

    [EbmlElement(0x466E, "FileName")]
    public string FileName { get; set; } = string.Empty;

    [EbmlElement(0x4660, "FileMimeType")]
    public string MimeType { get; set; } = string.Empty;

    [EbmlElement(0x465C, "FileData")]
    public byte[] Data { get; set; } = [];

    [EbmlElement(0x46AE, "FileUID")]
    public ulong FileUid { get; set; }
}