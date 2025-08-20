using System.Collections.Immutable;

namespace CapiGenerator.CModel.BuiltinMacroFunctions
{
    public static class AllBuiltinMacroFunctions
    {
        public static readonly ImmutableArray<BuiltinMacroFunctionBase> Functions = [
            new Uint32CMacroFunction()
        ];
    }
}
