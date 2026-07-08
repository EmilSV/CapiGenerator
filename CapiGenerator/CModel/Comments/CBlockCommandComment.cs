using System.Collections.Immutable;
using CppAst;

namespace CapiGenerator.CModel.Comments;

public sealed class CBlockCommandComment : CBaseComment
{
    public required string CommandName { get; init; }
    public required ImmutableArray<string> Arguments { get; init; }

    public static CBlockCommandComment From(CppCommentBlockCommand cppCommentBlockCommand)
    {
        return new CBlockCommandComment
        {
            CommandName = cppCommentBlockCommand.CommandName,
            Arguments = [.. cppCommentBlockCommand.Arguments],
            Children = [.. cppCommentBlockCommand.Children.Select(From)],
        };
    }
}
