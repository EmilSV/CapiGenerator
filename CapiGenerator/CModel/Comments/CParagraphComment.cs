using System.Collections.Immutable;
using CppAst;

namespace CapiGenerator.CModel.Comments;

public sealed class CParagraphComment : CBaseComment
{
    public static CParagraphComment From(CppCommentParagraph cppComment) => new()
    {
        Children = [.. cppComment.Children.Select(From)],
    };
}
