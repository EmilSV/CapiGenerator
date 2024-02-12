using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CppAst;

namespace CapiGenerator.Parser;

public class EnumParser : BaseParser
{
    public override void FirstPass(
        Guid compilationUnitId,
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel)
    {
        foreach (var compilation in compilations)
        {
            foreach (var enumValue in compilation.Enums)
            {
                if (enumValue == null || enumValue.Items.Count > 0)
                {
                    continue;
                }

                if (ShouldSkip(enumValue))
                {
                    continue;
                }

                var newConst = FirstPass(compilationUnitId, enumValue);
                if (newConst is not null)
                {
                    outputChannel.OnReceiveEnum(newConst);
                }
            }
        }
    }

    protected virtual CEnum? FirstPass(Guid compilationUnitId, CppEnum astEnum)
    {
        var enumConstants = astEnum.Items.Select(i => CppEnumItemToEnumFelid(compilationUnitId, i)).ToArray();
        if (enumConstants == null || enumConstants.Any(token => token is null))
        {
            OnError(astEnum, "Failed to parse tokens");
            return null;
        }

        return new CEnum(compilationUnitId, astEnum.Name, enumConstants!);
    }

    protected virtual bool ShouldSkip(CppEnum value) => false;
    protected virtual void OnError(CppEnum value, string message)
    {
        Console.Error.WriteLine($"Error parsing enum {value.Name}: {message}");
    }

    private static CEnumField? CppEnumItemToEnumFelid(Guid compilationUnitId, CppEnumItem item) =>
        new(compilationUnitId, item.Name, [
            new CConstLiteralToken(item.Value.ToString())
        ]);
}