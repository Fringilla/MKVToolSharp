using Xunit;
using System.IO;
using Fringilla.Matroska;
using System.Linq;

namespace Fringilla.Matroska.Tests;

public class EbmlRoundtripTests
{
    [Fact]
    public void Segment_Roundtrip_RetainsStructure()
    {
        var seg = new Segment
        {
            Info = new SegmentInfo
            {
                TimecodeScale = 1000000,
                Title = "PoC Test",
                MuxingApp = "fringilla-muxer",
                WritingApp = "fringilla-writer"
            },
            Tracks = new System.Collections.Generic.List<TrackEntry>
            {
                new TrackEntry
                {
                    TrackNumber = 1,
                    TrackType = 1,
                    CodecId = "V_MOCK",
                    Video = new VideoTrack { Width = 1280, Height = 720 }
                }
            },
            Clusters = new System.Collections.Generic.List<Cluster>
            {
                new Cluster
                {
                    Timecode = 0,
                    Blocks = new System.Collections.Generic.List<Block>
                    {
                        new Block { TrackNumber = 1, Timecode = 0, Keyframe = true, Data = new byte[] { 1,2,3,4 } }
                    }
                }
            },
            Attachments = new Attachments
            {
                Files = new System.Collections.Generic.List<AttachedFile>
                {
                    new AttachedFile { FileUid = 1, FileName = "readme.txt", MimeType = "text/plain", Data = System.Text.Encoding.UTF8.GetBytes("hello") }
                }
            }
        };

        var ser = new EbmlSerializer();
        var des = new EbmlDeserializer();
        using var ms = new MemoryStream();
        ser.Serialize(ms, seg);

        ms.Position = 0;
        var seg2 = des.Deserialize<Segment>(ms);

        Assert.NotNull(seg2.Info);
        Assert.Equal(seg.Info!.Title, seg2.Info!.Title);
        Assert.Single(seg2.Tracks);
        Assert.Single(seg2.Clusters);
        Assert.Single(seg2.Attachments!.Files);
        Assert.Equal("readme.txt", seg2.Attachments!.Files[0].FileName);
    }
}
