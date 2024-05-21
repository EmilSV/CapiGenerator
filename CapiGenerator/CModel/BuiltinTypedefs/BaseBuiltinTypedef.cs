using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public abstract class BaseBuiltinTypedef :
    BaseCAstItem, ICType
{
    public abstract string Name { get; }
    public abstract bool TypedefIsBuiltin(CppTypedef typedef);
}

