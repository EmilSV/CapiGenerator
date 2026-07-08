using CppAst;

namespace CapiGenerator.CModel.Comments;

public sealed class CParamCommandComment : CBaseComment
{
    public required string ParamName { get; init; }
    public required bool IsParamIndexValid { get; init; }
    public required int ParamIndex { get; init; }
    public required CppCommentParamDirection Direction { get; init; }
    public required bool IsDirectionExplicit { get; init; }

    public static CParamCommandComment From(CppCommentParamCommand cppComment) => new()
    {
        ParamName = cppComment.ParamName,
        IsParamIndexValid = cppComment.IsParamIndexValid,
        ParamIndex = cppComment.ParamIndex,
        Direction = cppComment.Direction,
        IsDirectionExplicit = cppComment.IsDirectionExplicit,
        Children = [.. cppComment.Children.Select(From)],
    };
}
