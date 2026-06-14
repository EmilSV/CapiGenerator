using CapiGenerator.CSModel.Comments;

namespace CapiGenerator.XmlComments;

public sealed record RemarkElement : SubCommentElementBase
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
            throw new NotImplementedException();
        }

        item.Comments ??= new DocComment();
        item.Comments.Remarks.Add(new()
        {
            Description = description
        });
    }
}
