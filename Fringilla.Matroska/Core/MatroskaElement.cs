namespace Fringilla.Matroska
{
    public abstract class MatroskaElement
    {
        public abstract string ElementName { get; }
        public abstract ulong ElementId { get; }

        // Optioneel: byte-offsets, grootte, etc.
        public long? Position { get; set; }
        public long? Size { get; set; }
    }
}