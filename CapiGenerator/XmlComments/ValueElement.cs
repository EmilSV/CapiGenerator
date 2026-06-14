using CapiGenerator.CSModel.Comments;

namespace CapiGenerator.XmlComments;

public sealed record ValueElement : SubCommentElementBase
{
    public override void AssignComment(IXmlCommentsTypeProvider typeProvider)
    {
        var item = XmlCommentFinder.FindComments(typeProvider, ApplyToLocation);
        if (item == null)
        {
            return;
        }

        string? description = Description;

        if (CloneFromLocation != null)
        {
            var newDescription = XmlCommentFinder.FindComments(typeProvider, CloneFromLocation)
                ?.Comments?.Value?.Description;

            if (newDescription != null)
            {
                description = newDescription;
            }
        }

        if (string.IsNullOrEmpty(description))
        {
            if (item.Comments?.Value == null)
            {
                return;
            }
            item.Comments.Value = null;
            return;
        }

        item.Comments ??= new DocComment();
        item.Comments.Value ??= new CommentValue();
        item.Comments.Value.Description = description;
    }
}
