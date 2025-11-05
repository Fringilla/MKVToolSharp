namespace Fringilla.Matroska;

public static class ElementId
{
    // === Top-level elements ===
    public const ulong Segment = 0x18538067;
    public const ulong SeekHead = 0x114D9B74;
    public const ulong Info = 0x1549A966;
    public const ulong Tracks = 0x1654AE6B;
    public const ulong Cluster = 0x1F43B675;
    public const ulong Cues = 0x1C53BB6B;
    public const ulong Chapters = 0x1043A770;
    public const ulong Attachments = 0x1941A469;
    public const ulong Tags = 0x1254C367;

    // === Info ===
    public const ulong TimecodeScale = 0x2AD7B1;
    public const ulong Duration = 0x4489;
    public const ulong Title = 0x7BA9;
    public const ulong MuxingApp = 0x4D80;
    public const ulong WritingApp = 0x5741;

    // === Tracks ===
    public const ulong TrackEntry = 0xAE;
    public const ulong TrackNumber = 0xD7;
    /// <summary>
    /// A human-readable track name.
    /// </summary>
    public const ulong TrackName = 0x536E;
    public const ulong TrackType = 0x83;
    public const ulong CodecId = 0x86;
    public const ulong CodecName = 0x258688;
    public const ulong Video = 0xE0;
    public const ulong Audio = 0xE1;

    // Video
    public const ulong PixelWidth = 0xB0;
    public const ulong PixelHeight = 0xBA;

    // Audio
    public const ulong SamplingFrequency = 0xB5;
    public const ulong Channels = 0x9F;

    // === Cluster ===
    public const ulong Timecode = 0xE7;
    public const ulong SimpleBlock = 0xA3;

    // === SeekHead ===
    public const ulong Seek = 0x4DBB;
    public const ulong SeekID = 0x53AB;
    public const ulong SeekPosition = 0x53AC;

    // === Cues ===
    public const ulong CuePoint = 0xBB;
    public const ulong CueTime = 0xB3;
    public const ulong CueTrackPositions = 0xB7;
    public const ulong CueTrack = 0xF7;
    public const ulong CueClusterPosition = 0xF1;

    // === Chapters ===
    public const ulong EditionEntry = 0x45B9;
    public const ulong ChapterAtom = 0xB6;
    public const ulong ChapterUID = 0x73C4;
    public const ulong ChapterTimeStart = 0x91;
    public const ulong ChapterTimeEnd = 0x92;
    public const ulong ChapterDisplay = 0x80;
    public const ulong ChapString = 0x85;
    public const ulong ChapLanguage = 0x437C;

    // === Attachments ===
    public const ulong AttachedFile = 0x61A7;
    public const ulong FileUID = 0x46AE;
    public const ulong FileName = 0x466E;
    public const ulong FileMimeType = 0x4660;
    public const ulong FileData = 0x465C;

    // === Tags ===
    public const ulong Tag = 0x7373;
    public const ulong Targets = 0x63C0;
    public const ulong TargetTypeValue = 0x63C5;
    public const ulong TargetType = 0x63CA;
    public const ulong TrackUID = 0x68CA;
    public const ulong EditionUID = 0x63C9;
    public const ulong ChapterUIDTarget = 0x63C4;
    public const ulong AttachmentUID = 0x63C6;
    public const ulong SimpleTag = 0x67C8;
    public const ulong TagName = 0x45A3;
    public const ulong TagLanguage = 0x447A;
    public const ulong TagString = 0x4487;
}
