using CapiGenerator.CModel.ConstantToken;

namespace CapiGenerator.CModel.BuiltinMacroFunctions
{
    public abstract class BuiltinMacroFunctionBase
    {
        public abstract string Name { get; }
        public abstract bool TryEvaluate(ReadOnlySpan<BaseCConstantToken> tokens, out List<BaseCConstantToken>? result);
    }
}