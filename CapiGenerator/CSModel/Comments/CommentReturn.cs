
namespace CapiGenerator.CSModel.Comments;

public sealed class CommentReturn
{
    public string? Description { get; set; }

    public bool HasValue()
    {
        return !string.IsNullOrEmpty(Description);
    }
}