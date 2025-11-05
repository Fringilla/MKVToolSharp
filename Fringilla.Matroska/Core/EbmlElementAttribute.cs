namespace Fringilla.Matroska;

/// <summary>
/// EBML element attribute used for serialization mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public sealed class EbmlElementAttribute : Attribute
{
    public ulong Id { get; }
    public string Name { get; }
    public int Order { get; init; } = int.MaxValue;
    
    public EbmlElementAttribute(ulong id, string name)
    {
        Id = id;
        Name = name;
    }
}