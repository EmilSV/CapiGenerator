using System.Collections.Immutable;
using CppAst;

namespace CapiGenerator.CModel.Comments;

public sealed class CVerbatimBlockCommandComment : CBaseComment
{
    public required string CommandName { get; init; }
    public required ImmutableArray<string> Arguments { get; init; }

    public static CVerbatimBlockCommandComment From(CppCommentVerbatimBlockCommand cppCommentText) => new()
    {
        CommandName = cppCommentText.CommandName,
        Arguments = [.. cppCommentText.Arguments],
        Children = [.. cppCommentText.Children.Select(From)],
    };
}
