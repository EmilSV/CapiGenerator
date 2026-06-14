using CapiGenerator.CSModel.Comments;

namespace CapiGenerator.XmlComments;

public sealed record SummaryElement : SubCommentElementBase
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
            var newDescription = XmlCommentFinder.FindComments(typeProvider, CloneFromLocation)?.Comments?.Summary?.Description;
            if (newDescription != null)
            {
                description = newDescription;
            }
        }

        item.Comments ??= new DocComment();
        item.Comments.Summary ??= new CommentSummery();
        item.Comments.Summary.Description = description;
    }
}
