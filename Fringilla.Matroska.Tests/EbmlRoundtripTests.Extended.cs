namespace Fringilla.Matroska.Tests;

public partial class EbmlRoundtripTests
{
    [Fact]
    public void Roundtrip_SignedIntegers_ArePreserved()
    {
        var info = new SegmentInfo
        {
            Title = "SignedTest",
            TimecodeScale = 1000000,
            Duration = 5.0
        };

        var seg = new Segment { Info = info };

        using var ms = new MemoryStream();
        EbmlSerializer.Serialize(ms, seg);
        ms.Position = 0;

        var copy = new EbmlDeserializer().Deserialize<Segment>(ms);

        Assert.NotNull(copy.Info);
        Assert.Equal("SignedTest", copy.Info!.Title);
        Assert.Equal(1000000UL, copy.Info.TimecodeScale);
        Assert.Equal(5.0, (double)copy.Info.Duration!, 3);
    }

    [Fact]
    public void Roundtrip_FloatingPointPrecision_IsWithinTolerance()
    {
        var info = new SegmentInfo { Duration = 9876.54321 };
        var seg = new Segment { Info = info };

        using var ms = new MemoryStream();
        EbmlSerializer.Serialize(ms, seg);
        ms.Position = 0;

        var copy = new EbmlDeserializer().Deserialize<Segment>(ms);

        Assert.NotNull(copy.Info);
        Assert.InRange(copy.Info!.Duration!.Value, 9876.54, 9876.55);
    }

    [Fact]
    public void Roundtrip_NestedElements_ClustersAndBlocks()
    {
        var seg = new Segment
        {
            Clusters =
            {
                new Cluster
                {
                    Timecode = 0,
                    Blocks = { new Block { TrackNumber = 1, Data = new byte[] { 0x01, 0x02 } } }
                },
                new Cluster
                {
                    Timecode = 1000,
                    Blocks = { new Block { TrackNumber = 2, Data = new byte[] { 0xFF } } }
                }
            }
        };

        using var ms = new MemoryStream();
        EbmlSerializer.Serialize(ms, seg);
        ms.Position = 0;

        var copy = new EbmlDeserializer().Deserialize<Segment>(ms);

        Assert.Equal(2, copy.Clusters.Count);
        Assert.Equal(0UL, copy.Clusters[0].Timecode);
        Assert.Equal(1000UL, copy.Clusters[1].Timecode);
        Assert.Equal(1, copy.Clusters[0].Blocks[0].TrackNumber);
        Assert.Equal(2, copy.Clusters[1].Blocks[0].TrackNumber);
    }

    [Fact]
    public void Roundtrip_UnknownElements_AreIgnored()
    {
        var seg = new Segment
        {
            Info = new SegmentInfo { Title = "IgnoreUnknown" },
            Tracks = { new TrackEntry { TrackNumber = 1, TrackName = "Main" } }
        };

        using var ms = new MemoryStream();
        EbmlSerializer.Serialize(ms, seg);

        // Voeg 'fictief onbekend element' toe aan de stream
        var bogusId = EbmlBinary.IdToBytes(0x0F00FF);
        var bogusSize = EbmlBinary.EncodeVInt(2);
        ms.Write(bogusId, 0, bogusId.Length);
        ms.Write(bogusSize, 0, bogusSize.Length);
        ms.WriteByte(0xDE);
        ms.WriteByte(0xAD);

        ms.Position = 0;
        var copy = new EbmlDeserializer().Deserialize<Segment>(ms);

        Assert.NotNull(copy.Info);
        Assert.Equal("IgnoreUnknown", copy.Info!.Title);
        Assert.Single(copy.Tracks);
    }

    [Fact]
    public void Handles_EmptyCollections_AndNullables()
    {
        var seg = new Segment
        {
            Info = new SegmentInfo { Title = "EmptyTest", Duration = null },
            Tracks = new()
        };

        using var ms = new MemoryStream();
        EbmlSerializer.Serialize(ms, seg);
        ms.Position = 0;

        var copy = new EbmlDeserializer().Deserialize<Segment>(ms);

        Assert.NotNull(copy.Info);
        Assert.Null(copy.Info!.Duration);
        Assert.Empty(copy.Tracks);
    }
}