using System;
using CapiGenerator.Extensions;

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
        foreach (var item in Description?.SplitNewLine() ?? [])
        {
            writer.WriteLine($"/// {item}");
        }
        writer.WriteLine("/// </summary>");
    }
}