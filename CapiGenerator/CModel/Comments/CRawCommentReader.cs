using System.Collections.Concurrent;
using System.Collections.Immutable;
using CppAst;

namespace CapiGenerator.CModel.Comments;

/// <summary>
/// Reads the doc comment directly above a declaration from its source file.
/// </summary>
public static class CRawCommentReader
{
    private static readonly ConcurrentDictionary<string, string[]> FileLines = new();

    public static bool IsCodeFence(string text) => text.TrimStart().StartsWith("```", StringComparison.Ordinal);

    /// <summary>
    /// Returns the content of each fenced (```) code block in the doc comment above the declaration,
    /// with indentation preserved, or an empty array if no doc comment is found.
    /// </summary>
    public static ImmutableArray<string> ReadCodeBlocks(CppSourceSpan span)
    {
        var commentLines = ReadCommentLines(span);
        if (commentLines is null)
        {
            return [];
        }

        var codeBlocks = ImmutableArray.CreateBuilder<string>();
        List<string>? codeLines = null;
        var fenceIndent = 0;

        foreach (var line in commentLines)
        {
            if (IsCodeFence(line))
            {
                if (codeLines is null)
                {
                    codeLines = [];
                    fenceIndent = line.Length - line.TrimStart().Length;
                }
                else
                {
                    codeBlocks.Add(string.Join('\n', codeLines));
                    codeLines = null;
                }

                continue;
            }

            codeLines?.Add(RemoveIndent(line, fenceIndent).TrimEnd());
        }

        return codeBlocks.ToImmutable();
    }

    private static List<string>? ReadCommentLines(CppSourceSpan span)
    {
        var file = span.Start.File;
        if (string.IsNullOrEmpty(file) || span.Start.Line < 2 || !File.Exists(file))
        {
            return null;
        }

        var lines = FileLines.GetOrAdd(Path.GetFullPath(file), File.ReadAllLines);

        // The doc comment ends on the closest non-blank line above the declaration.
        var end = span.Start.Line - 2;
        while (end >= 0 && string.IsNullOrWhiteSpace(lines[end]))
        {
            end--;
        }

        if (end < 0 || !lines[end].TrimEnd().EndsWith("*/", StringComparison.Ordinal))
        {
            return null;
        }

        var start = end;
        while (start >= 0 && !lines[start].Contains("/**") && !lines[start].Contains("/*!"))
        {
            start--;
        }

        if (start < 0)
        {
            return null;
        }

        var commentLines = new List<string>();
        for (var i = start; i <= end; i++)
        {
            var line = lines[i];
            if (i == start)
            {
                line = line[(line.IndexOf("/*", StringComparison.Ordinal) + 3)..];
            }

            if (i == end)
            {
                line = line[..line.LastIndexOf("*/", StringComparison.Ordinal)];
            }

            commentLines.Add(RemoveDecoration(line));
        }

        return commentLines;
    }

    // Strips the leading " * " of a comment line, keeping any indentation after it.
    private static string RemoveDecoration(string line)
    {
        var trimmed = line.TrimStart();
        if (!trimmed.StartsWith('*'))
        {
            return line;
        }

        trimmed = trimmed[1..];
        return trimmed.StartsWith(' ') ? trimmed[1..] : trimmed;
    }

    private static string RemoveIndent(string line, int indent)
    {
        var whitespace = line.Length - line.TrimStart().Length;
        return line[Math.Min(indent, whitespace)..];
    }
}
