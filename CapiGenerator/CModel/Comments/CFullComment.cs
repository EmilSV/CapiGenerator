using CppAst;

namespace CapiGenerator.CModel.Comments;

public sealed class CFullComment : CBaseComment
{
    public static CFullComment From(CppCommentFull cppComment)
    {
        return new CFullComment
        {
            Children = [.. cppComment.Children.Select(From)],
        };
    }
}
