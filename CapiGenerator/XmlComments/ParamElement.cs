using CapiGenerator.CSModel.Comments;

namespace CapiGenerator.XmlComments;

public sealed record ParamElement : SubCommentElementBase
{
    public required string? Name { get; init; }

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
                ?.Comments?.Parameters.FirstOrDefault(i => i.Name == Name)?.Description;

            if (newDescription != null)
            {
                description = newDescription;
            }
        }

        if (string.IsNullOrEmpty(description))
        {
            item.Comments?.Parameters.RemoveAll(i => i.Name == Name);
            return;
        }


        item.Comments ??= new DocComment();
        var param = item.Comments.Parameters.FirstOrDefault(i => i.Name == Name);
        if (param == null)
        {
            param = new()
            {
                Name = Name!,
                Description = description!
            };
            item.Comments.Parameters.Add(param);
        }
        else
        {
            param.Description = description!;
        }
    }
}
