using System;

namespace CapiGenerator.CSModel.Comments;

public sealed class CommentSummery
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

        writer.WriteLine("/// <summary>");

        var descriptionSpan = Description.AsSpan();
        foreach (var range in descriptionSpan.Split('\n'))
        {
            writer.WriteLine($"/// {descriptionSpan[range]}");
        }

        writer.WriteLine("/// </summary>");
    }
}