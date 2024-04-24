using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.ConstantToken;

public class CSConstIdentifierToken : BaseCSConstantToken
{
    private readonly ResoleRef<ICSFieldLike, ICConstAssignable> _constantField;

    public CSConstIdentifierToken(ICConstAssignable constAssignable)
    {
        _constantField = new(constAssignable);
    }

    public CSConstIdentifierToken(ICSFieldLike field)
    {
        _constantField = new(field);
    }

    public ICSFieldLike? GetField()
    {
        return _constantField.Output;
    }

    public override void OnSecondPass(CSTranslationUnit translationUnit)
    {
        if (_constantField.IsNull || _constantField.IsOutputResolved())
        {
            return;
        }

        _constantField.TrySetOutputFromResolver(translationUnit);
    }

    public static CSConstIdentifierToken FromCConstantToken(CConstIdentifierToken token)
    {
        return new CSConstIdentifierToken(token.GetConstantModel() ?? throw new InvalidOperationException("Constant model is not resolved"));
    }

    public override string? ToString()
    {
        var output = _constantField.Output;
        if (output is CSEnumField enumField)
        {
            return enumField.ParentEnum?.Type?.KindValue switch
            {
                CSPrimitiveType.Kind.Byte => $"(byte){enumField.GetFullName()}",
                CSPrimitiveType.Kind.SByte => $"(sbyte){enumField.GetFullName()}",
                CSPrimitiveType.Kind.Short => $"(short){enumField.GetFullName()}",
                CSPrimitiveType.Kind.UShort => $"(ushort){enumField.GetFullName()}",
                CSPrimitiveType.Kind.Int => $"(int){enumField.GetFullName()}",
                CSPrimitiveType.Kind.UInt => $"(uint){enumField.GetFullName()}",
                CSPrimitiveType.Kind.Long => $"(long){enumField.GetFullName()}",
                CSPrimitiveType.Kind.ULong => $"(ulong){enumField.GetFullName()}",
                CSPrimitiveType.Kind.NInt => $"(nint){enumField.GetFullName()}",
                CSPrimitiveType.Kind.NUInt => $"(nuint){enumField.GetFullName()}",
                _ => throw new InvalidOperationException("Unknown enum type")
            };
        }
        return output?.GetFullName();
    }
}
