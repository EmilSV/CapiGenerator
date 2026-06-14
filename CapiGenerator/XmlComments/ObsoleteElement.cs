using CapiGenerator.CSModel;

namespace CapiGenerator.XmlComments;


public record ObsoleteElement : SubElementBase
{
    public string? Message { get; init; }
    public required string ApplyToLocation { get; init; }
    public bool IsError { get; init; } = true;

    public override void AssignComment(IXmlCommentsTypeProvider typeProvider)
    {
        var item = XmlCommentFinder.FindComments(typeProvider, ApplyToLocation);
        if (item is IAttributeAssignableItem attributeAssignableItem)
        {
            string finalMessage = Message ?? string.Empty;

            finalMessage = finalMessage.Replace("\"", "\\\"");

            attributeAssignableItem.Attributes.Add(
                CSAttribute<ObsoleteAttribute>.Create(
                   [$"\"{finalMessage}\"", IsError ? "true" : "false"],
                   []
                )
            );
        }
    }
}
