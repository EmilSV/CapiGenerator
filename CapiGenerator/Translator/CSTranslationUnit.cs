using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel;
using CapiGenerator.CSModel;

namespace CapiGenerator.Translator;

public sealed class CSTranslationUnit : 
    IResolver<ICSType, ICType>,
    IResolver<CSField, BaseCAstItem>
{

    ICSType? IResolver<ICSType, ICType>.Resolve(ICType key)
    {
        throw new NotImplementedException();
    }

    CSField? IResolver<CSField, BaseCAstItem>.Resolve(BaseCAstItem key)
    {
        throw new NotImplementedException();
    }
}