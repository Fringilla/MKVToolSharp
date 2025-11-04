namespace Fringilla.Matroska
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class EbmlElementAttribute : Attribute
    {
        public ulong Id { get; }
        public string Name { get; }
        public EbmlElementAttribute(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}