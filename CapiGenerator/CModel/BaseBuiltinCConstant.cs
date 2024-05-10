using CppAst;

namespace CapiGenerator.CModel;

public abstract class BaseBuiltInCConstant :
    BaseCConstant
{
    public abstract bool MacroIsBuiltin(CppMacro macro);
}