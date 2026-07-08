using System.Collections.Immutable;
using CppAst;

namespace CapiGenerator.CModel.Comments;

public sealed class CHtmlStartTagComment : CBaseComment
{
    public required string TagName { get; init; }
    public required bool IsSelfClosing { get; init; }
    public required ImmutableArray<KeyValuePair<string, string>> Attributes { get; init; }

    public static CHtmlStartTagComment From(CppCommentHtmlStartTag cppComment)
    {
        return new CHtmlStartTagComment
        {
            TagName = cppComment.TagName,
            IsSelfClosing = cppComment.IsSelfClosing,
            Attributes = [.. cppComment.Attributes],
            Children = [.. cppComment.Children.Select(From)],
        };
    }
}
