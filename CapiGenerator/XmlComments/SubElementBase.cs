namespace CapiGenerator.XmlComments;

public abstract record SubElementBase
{
    public required int Priority { get; init; }
    public abstract void AssignComment(IXmlCommentsTypeProvider typeProvider);
}
