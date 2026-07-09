using System.Collections.Immutable;
using CppAst;

namespace CapiGenerator.CModel.Comments;


public abstract class CBaseComment
{
    public ImmutableArray<CBaseComment> Children { get; init; } = ImmutableArray<CBaseComment>.Empty;

    public static CBaseComment From(CppComment cppCommentText) => cppCommentText switch
    {
        CppCommentBlockCommand blockCommand => CBlockCommandComment.From(blockCommand),
        CppCommentFull commentFull => CFullComment.From(commentFull),
        CppCommentHtmlEndTag htmlEndTag => CHtmlEndTagComment.From(htmlEndTag),
        CppCommentHtmlStartTag htmlStartTagComment => CHtmlStartTagComment.From(htmlStartTagComment),
        CppCommentInlineCommand inlineCommandComment => CInlineCommandComment.From(inlineCommandComment),
        CppCommentParagraph paragraphComment => CParagraphComment.From(paragraphComment),
        CppCommentParamCommand paramCommand => CParamCommandComment.From(paramCommand),
        CppCommentTemplateParamCommand templateParamCommand => CTemplateParamCommandComment.From(templateParamCommand),
        CppCommentText commentText => CTextComment.From(commentText),
        CppCommentVerbatimBlockCommand commentVerbatimBlockCommand => CVerbatimBlockCommandComment.From(commentVerbatimBlockCommand),
        CppCommentVerbatimBlockLine commentVerbatimBlockLine => CVerbatimBlockLineComment.From(commentVerbatimBlockLine),
        CppCommentVerbatimLine commentVerbatimLine => CVerbatimLineComment.From(commentVerbatimLine),
        _ => throw new NotImplementedException("CppComment type not supported: " + cppCommentText.GetType().FullName)
    };
}
