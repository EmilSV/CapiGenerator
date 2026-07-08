using System.Collections.Immutable;
using CppAst;

namespace CapiGenerator.CModel.Comments;


public sealed class CInlineCommandComment : CBaseComment
{
    public enum RenderKinds
    {
        Normal,
        Bold,
        Monospaced,
        Emphasized
    }

    public RenderKinds RenderKind { get; init; }
    public required string CommandName { get; init; }
    public required ImmutableArray<string> Arguments { get; init; }


    public static CInlineCommandComment From(CppCommentInlineCommand cppComment)
    {
        return new CInlineCommandComment
        {
            RenderKind = cppComment.RenderKind switch
            {
                CppCommentInlineCommandRenderKind.Normal => RenderKinds.Normal,
                CppCommentInlineCommandRenderKind.Bold => RenderKinds.Bold,
                CppCommentInlineCommandRenderKind.Monospaced => RenderKinds.Monospaced,
                CppCommentInlineCommandRenderKind.Emphasized => RenderKinds.Emphasized,
                _ => RenderKinds.Normal,
            },
            CommandName = cppComment.CommandName,
            Arguments = [.. cppComment.Arguments],
        };
    }
}
