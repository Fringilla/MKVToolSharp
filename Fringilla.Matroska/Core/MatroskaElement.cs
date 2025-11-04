namespace Fringilla.Matroska
{
    public abstract class MatroskaElement
    {
        public abstract string Name { get; }
        public abstract ulong Id { get; }

        // Optioneel: byte-offsets, grootte, etc.
        public long? Position { get; set; }
        public long? Size { get; set; }
    }
}