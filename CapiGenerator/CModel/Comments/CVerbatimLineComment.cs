using CppAst;

namespace CapiGenerator.CModel.Comments;

public sealed class CVerbatimLineComment : CBaseComment
{
    public required string Text { get; init; }

    public static CVerbatimLineComment From(CppCommentVerbatimLine cppComment) => new()
    {
        Text = cppComment.Text,
    };
}
