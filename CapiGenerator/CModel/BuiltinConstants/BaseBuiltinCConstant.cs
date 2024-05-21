using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public abstract class BaseBuiltInCConstant :
    BaseCConstant
{
    public abstract bool MacroIsBuiltin(CppMacro macro);
}