using System.Collections.Immutable;

namespace CapiGenerator.CModel.BuiltinMacroFunctions
{
    public static class AllBuiltinMacroFunctions
    {
        public static readonly ImmutableArray<BuiltinMacroFunctionBase> Functions = [
            new Int64CMacroFunction(),
            new UInt64CMacroFunction(),
            new Uint32CMacroFunction()
        ];
    }
}
