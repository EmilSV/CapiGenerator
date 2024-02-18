using CapiGenerator.CModel;
using CapiGenerator.CSModel;

namespace CapiGenerator.Translator;

public sealed class CSTranslationUnit : IResolver<ICSType, ICType>
{
    ICSType? IResolver<ICSType, ICType>.Resolve(ICType key)
    {
        throw new NotImplementedException();
    }
}