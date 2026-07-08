using System.Collections.Immutable;
using CppAst;

namespace CapiGenerator.CModel.Comments;

public sealed class CHtmlEndTagComment : CBaseComment
{
    public required string TagName { get; init; }
    public static CHtmlEndTagComment From(CppCommentHtmlEndTag cppComment)
    {
        return new CHtmlEndTagComment
        {
            TagName = cppComment.TagName,
            Children = [.. cppComment.Children.Select(From)],
        };
    }
}
