

namespace CapiGenerator.CSModel.Comments;

public sealed class CommentValue
{
    public string? Description { get; set; }

    public bool HasValue()
    {
        return !string.IsNullOrEmpty(Description);
    }

    public void WriteToStream(StreamWriter writer)
    {
        if (!HasValue())
        {
            return;
        }

        writer.WriteLine("/// <value>");
        var descriptionSpan = Description.AsSpan();
        foreach (var range in descriptionSpan.Split('\n'))
        {
            writer.WriteLine($"/// {descriptionSpan[range]}");
        }
        writer.WriteLine("/// </value>");
    }
}