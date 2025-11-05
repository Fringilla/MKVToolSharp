namespace Fringilla.Matroska.Tests;

public partial class EbmlRoundtripTests
{
    [Fact]
    public void CanSerializeAndDeserialize_PrimitiveSegment()
    {
        var seg = new Segment
        {
            Info = new SegmentInfo
            {
                Title = "UnitTest",
                Duration = 123.45
            }
        };

        using var ms = new MemoryStream();
        EbmlSerializer.Serialize(ms, seg);
        ms.Position = 0;

        var des = new EbmlDeserializer();
        var copy = des.Deserialize<Segment>(ms);

        Assert.NotNull(copy.Info);
        Assert.Equal("UnitTest", copy.Info.Title);
        Assert.Equal(123.45, copy.Info.Duration ?? 0, 3);
    }

    [Fact]
    public void CanSerializeAndDeserialize_ListElements()
    {
        var seg = new Segment
        {
            Tracks = new()
            {
                new TrackEntry { TrackNumber = 1, TrackName = "Video" },
                new TrackEntry { TrackNumber = 2, TrackName = "Audio" }
            }
        };

        using var ms = new MemoryStream();
        EbmlSerializer.Serialize(ms, seg);
        ms.Position = 0;

        var des = new EbmlDeserializer();
        var copy = des.Deserialize<Segment>(ms);

        Assert.Equal(2, copy.Tracks.Count);
        Assert.Equal("Video", copy.Tracks[0].TrackName);
        Assert.Equal("Audio", copy.Tracks[1].TrackName);
    }

    [Fact]
    public void HandlesEmptyOptionalElements()
    {
        var seg = new Segment();
        using var ms = new MemoryStream();
        EbmlSerializer.Serialize(ms, seg);
        ms.Position = 0;

        var des = new EbmlDeserializer();
        var copy = des.Deserialize<Segment>(ms);

        Assert.Null(copy.Info);
        Assert.Empty(copy.Tracks);
    }
}