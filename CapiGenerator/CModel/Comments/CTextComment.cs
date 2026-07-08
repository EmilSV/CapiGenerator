using CppAst;

namespace CapiGenerator.CModel.Comments;

public sealed class CTextComment : CBaseComment
{
    public required string Text { get; init; }

    public static CTextComment From(CppCommentText cppCommentText) => new()
    {
        Text = cppCommentText.Text,
        Children = [.. cppCommentText.Children?.Select(CBaseComment.From) ?? []]
    };
}
