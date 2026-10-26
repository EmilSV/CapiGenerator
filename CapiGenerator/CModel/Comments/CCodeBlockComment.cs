using System.Collections.Immutable;

namespace CapiGenerator.CModel.Comments;

/// <summary>
/// A fenced (```) code block. Clang has no notion of these, so they are regrouped from its
/// paragraphs by <see cref="CFullComment"/>.
/// </summary>
public sealed class CCodeBlockComment : CBaseComment
{
    public required ImmutableArray<string> Lines { get; init; }
}
