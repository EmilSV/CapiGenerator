using System.Collections.Immutable;
using CppAst;

namespace CapiGenerator.CModel.Comments;


public sealed class CTemplateParamCommandComment : CBaseComment
{
    public required string ParamName { get; init; }
    public required int Depth { get; init; }
    public required bool IsPositionValid { get; init; }
    public required int Index { get; init; }
    public required string CommandName { get; init; }
    public required ImmutableArray<string> Arguments { get; init; }

    public static CTemplateParamCommandComment From(CppCommentTemplateParamCommand cppComment) => new()
    {
        ParamName = cppComment.ParamName,
        Depth = cppComment.Depth,
        IsPositionValid = cppComment.IsPositionValid,
        Index = cppComment.Index,
        CommandName = cppComment.ParamName,
        Arguments = [.. cppComment.Arguments],
        Children = [.. cppComment.Children.Select(From)],
    };
}
