using System.Diagnostics.CodeAnalysis;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseCSAnonymousType : ICSType, ICSSecondPassable, ITypeReplace
{
    public BaseCSAnonymousType()
    {
        Name = $"__AnonymousType{Id}__";
    }

    public InstanceId Id { get; } = new();

    public string? Namespace => null;

    public string Name { get; }
    
    public bool IsAnonymous => true;

    public virtual void OnSecondPass(CSTranslationUnit unit)
    { }

    public abstract string GetFullTypeDefString();
    
    public abstract void ReplaceTypes(ITypeReplace.ReplacePredicate predicate);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = null;
        return false;
    }
}