using CapiGenerator.CModel.Type;

namespace CapiGenerator.CSModel;

public abstract class BaseCToCSPrimitiveTypeMapper
{
    public abstract IEnumerable<CPrimitiveType.Kind> GetSupportedTypes();
    public abstract CSPrimitiveType.Kind GetCSTypes(CPrimitiveType.Kind cPrimitiveType);
}