using System.Collections.Generic;

namespace Fringilla.Matroska
{
    [EbmlElement(0x1F43B675, "Cluster")]
    public sealed class Cluster : MatroskaElement
    {
        public override string Name => "Cluster";
        public override ulong Id => 0x1F43B675;

        [EbmlElement(0xE7, "Timecode")]
        public ulong Timecode { get; set; }

        [EbmlElement(0xA3, "SimpleBlock")]
        public List<Block> Blocks { get; set; } = new();
    }
}