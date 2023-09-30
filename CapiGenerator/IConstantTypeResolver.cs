using CapiGenerator.Model;

namespace CapiGenerator;

public interface IConstantTypeResolver
{
    public ConstantType ResolveType(Constant constant);
}