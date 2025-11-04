namespace Fringilla.Matroska;

[EbmlElement(0x1254C367, "Tags")]
public sealed class Tags : MatroskaElement
{
    public override string Name => "Tags";
    public override ulong Id => 0x1254C367;

    [EbmlElement(0x7373, "Tag")]
    public List<Tag> Items { get; set; } = new();
}

[EbmlElement(0x7373, "Tag")]
public sealed class Tag : MatroskaElement
{
    public override string Name => "Tag";
    public override ulong Id => 0x7373;

    [EbmlElement(0x63C0, "Targets")]
    public TagTarget? Target { get; set; }

    [EbmlElement(0x67C8, "SimpleTag")]
    public List<SimpleTag> SimpleTags { get; set; } = new();
}

[EbmlElement(0x63C0, "Targets")]
public sealed class TagTarget : MatroskaElement
{
    public override string Name => "Targets";
    public override ulong Id => 0x63C0;

    [EbmlElement(0x63C5, "TargetTypeValue")]
    public int? TargetTypeValue { get; set; }

    [EbmlElement(0x63CA, "TargetType")]
    public string? TargetType { get; set; }

    [EbmlElement(0x68CA, "TrackUID")]
    public ulong? TrackUid { get; set; }

    [EbmlElement(0x63C9, "EditionUID")]
    public ulong? EditionUid { get; set; }

    [EbmlElement(0x63C4, "ChapterUID")]
    public ulong? ChapterUid { get; set; }

    [EbmlElement(0x63C6, "AttachmentUID")]
    public ulong? AttachmentUid { get; set; }
}

[EbmlElement(0x67C8, "SimpleTag")]
public sealed class SimpleTag : MatroskaElement
{
    public override string Name => "SimpleTag";
    public override ulong Id => 0x67C8;

    [EbmlElement(0x45A3, "TagName")]
    public string NameValue { get; set; } = string.Empty;

    [EbmlElement(0x447A, "TagLanguage")]
    public string? Language { get; set; }

    [EbmlElement(0x4487, "TagString")]
    public string? Value { get; set; }
}