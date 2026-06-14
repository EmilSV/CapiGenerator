using CapiGenerator.CSModel.Comments;

namespace CapiGenerator.XmlComments;

public sealed record ReturnElement : SubCommentElementBase
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
                ?.Comments?.Return?.Description;

            if (newDescription != null)
            {
                description = newDescription;
            }
        }

        if (string.IsNullOrEmpty(description))
        {
            if (item.Comments?.Return == null)
            {
                return;
            }
            item.Comments.Return = null;
            return;
        }

        item.Comments ??= new DocComment();
        item.Comments.Return ??= new CommentReturn();
        item.Comments.Return.Description = description;
    }
}
