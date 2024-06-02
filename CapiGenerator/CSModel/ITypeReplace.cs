namespace CapiGenerator.CSModel;

public interface ITypeReplace
{
    public delegate bool ReplacePredicate(ICSType type, out ICSType newType);

    void ReplaceTypes(ReplacePredicate predicate);
}