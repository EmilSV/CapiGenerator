using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;

namespace CapiGenerator.CModel.BuiltinMacroFunctions;

public class Int64CMacroFunction : BuiltinMacroFunctionBase
{
    public override string Name => "INT64_C";

    public override bool TryEvaluate(
        IReadOnlyList<IReadOnlyList<BaseCConstantToken>> arguments,
        out List<BaseCConstantToken>? result)
    {
        if (arguments is not [var argument] || argument.Count == 0 ||
            argument.OfType<CConstLiteralToken>().Any(literal =>
                literal.Type is CConstantType.Float or CConstantType.Double or CConstantType.String or CConstantType.Char or CConstantType.Unknown))
        {
            result = null;
            return false;
        }

        if (argument is [CConstLiteralToken literalToken])
        {
            result =
            [
                new CConstLiteralToken(literalToken.Value, CConstantType.Int64_t, literalToken.SourceLocation)
            ];
            return true;
        }

        var sourceLocation = argument[0].SourceLocation;
        result =
        [
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.LeftParenthesis },
            new CConstCastToken(CPrimitiveType.Instances.LongLong, sourceLocation),
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.LeftParenthesis },
            .. argument,
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.RightParenthesis },
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.RightParenthesis }
        ];
        return true;
    }
}
