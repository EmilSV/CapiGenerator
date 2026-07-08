using CppAst;

namespace CapiGenerator.CModel.Comments;

public sealed class CVerbatimBlockLineComment : CBaseComment
{
    public required string Text { get; init; }

    public static CVerbatimBlockLineComment From(CppCommentVerbatimBlockLine cppComment) => new()
    {
        Text = cppComment.Text,
        Children = [.. cppComment.Children.Select(From)]
    };
}
