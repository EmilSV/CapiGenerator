using System.Collections.Immutable;
using CppAst;

namespace CapiGenerator.CModel.Comments;

public sealed class CFullComment : CBaseComment
{
    public static CFullComment From(CppCommentFull cppComment)
    {
        return new CFullComment
        {
            Children = [.. cppComment.Children.Select(From)],
        };
    }

    public static CFullComment From(CppCommentFull cppComment, CppSourceSpan span)
    {
        var comment = From(cppComment);
        var fenceCount = comment.Children.Sum(CountCodeFences);
        if (fenceCount == 0)
        {
            return comment;
        }

        // CppAst strips the indentation of comment text, so prefer the code blocks from the source,
        // but only if they match the fences clang found.
        var sourceCodeBlocks = CRawCommentReader.ReadCodeBlocks(span);
        if (sourceCodeBlocks.Length * 2 != fenceCount)
        {
            sourceCodeBlocks = [];
        }

        return new CFullComment
        {
            Children = GroupCodeBlocks(comment.Children, sourceCodeBlocks),
        };
    }

    // Clang splits a fenced code block into several paragraphs when it contains blank lines.
    // Replace the text between each pair of top-level fences with a single code block node.
    private static ImmutableArray<CBaseComment> GroupCodeBlocks(
        ImmutableArray<CBaseComment> children,
        ImmutableArray<string> sourceCodeBlocks)
    {
        var result = ImmutableArray.CreateBuilder<CBaseComment>();
        var nextSourceCodeBlock = 0;
        List<string>? parsedLines = null;

        foreach (var child in children)
        {
            if (child is not CParagraphComment paragraph)
            {
                result.Add(child);
                continue;
            }

            var paragraphChildren = new List<CBaseComment>();
            foreach (var node in paragraph.Children)
            {
                if (node is CTextComment fence && CRawCommentReader.IsCodeFence(fence.Text))
                {
                    if (parsedLines is null)
                    {
                        AddParagraph(result, paragraphChildren);
                        parsedLines = [];
                        continue;
                    }

                    var lines = nextSourceCodeBlock < sourceCodeBlocks.Length
                        ? sourceCodeBlocks[nextSourceCodeBlock++].Split('\n')
                        : parsedLines.ToArray();
                    result.Add(new CCodeBlockComment { Lines = [.. lines] });
                    parsedLines = null;
                }
                else if (parsedLines is not null)
                {
                    if (node is CTextComment text)
                    {
                        parsedLines.Add(text.Text.Trim());
                    }
                }
                else
                {
                    paragraphChildren.Add(node);
                }
            }

            AddParagraph(result, paragraphChildren);
        }

        if (parsedLines is not null)
        {
            result.Add(new CCodeBlockComment { Lines = [.. parsedLines] });
        }

        return result.ToImmutable();
    }

    private static void AddParagraph(ImmutableArray<CBaseComment>.Builder result, List<CBaseComment> children)
    {
        if (children.Count > 0)
        {
            result.Add(new CParagraphComment { Children = [.. children] });
            children.Clear();
        }
    }

    private static int CountCodeFences(CBaseComment comment) =>
        comment is CParagraphComment paragraph
            ? paragraph.Children.Count(node => node is CTextComment text && CRawCommentReader.IsCodeFence(text.Text))
            : 0;
}
