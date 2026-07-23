using CapiGenerator.CModel.ConstantToken;

namespace CapiGenerator.CModel.BuiltinMacroFunctions;

public class Int64CMacroFunction : BuiltinMacroFunctionBase
{
    public override string Name => "INT64_C";

    public override bool TryEvaluate(ReadOnlySpan<BaseCConstantToken> tokens, out List<BaseCConstantToken>? result)
    {
        if (tokens is not [CConstLiteralToken literalToken] ||
            literalToken.Type is CConstantType.Float or CConstantType.Double or CConstantType.String or CConstantType.Char or CConstantType.Unknown)
        {
            result = null;
            return false;
        }

        result =
        [
            new CConstLiteralToken(literalToken.Value, CConstantType.Int64_t, literalToken.SourceLocation)
        ];
        return true;
    }
}
