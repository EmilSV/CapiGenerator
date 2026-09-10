using CapiGenerator.CModel.ConstantToken;

namespace CapiGenerator.CModel.BuiltinMacroFunctions
{
    public abstract class BuiltinMacroFunctionBase
    {
        public abstract string Name { get; }
        public abstract bool TryEvaluate(
            IReadOnlyList<IReadOnlyList<BaseCConstantToken>> arguments,
            out List<BaseCConstantToken>? result);
    }
}