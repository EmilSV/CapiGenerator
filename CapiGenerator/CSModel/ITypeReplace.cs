using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator.CSModel;

public interface ITypeReplace
{
    public delegate bool ReplacePredicate(ICSType type, [NotNullWhen(true)] out ICSType? newType);

    void ReplaceTypes(ReplacePredicate predicate);

    public static void ReplaceTypes(IEnumerable<ITypeReplace> typeReplace, ReplacePredicate predicate)
    {
        foreach (var typeReplacer in typeReplace)
        {
            typeReplacer.ReplaceTypes(predicate);
        }
    }
}

public static class ITypeReplaceExtensions
{
    public static void ReplaceTypes(this IEnumerable<ITypeReplace> typeReplace, ITypeReplace.ReplacePredicate predicate)
    {
        foreach (var typeReplacer in typeReplace)
        {
            typeReplacer.ReplaceTypes(predicate);
        }
    }

    public static void ReplaceTypes(this IEnumerable<IEnumerable<ITypeReplace>> typeReplacers, ITypeReplace.ReplacePredicate predicate)
    {
        foreach (var typeReplacersItem in typeReplacers)
        {
            foreach (var item in typeReplacersItem)
            {
                item.ReplaceTypes(predicate);
            }
        }
    }
}