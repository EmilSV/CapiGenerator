using CapiGenerator.Model;

namespace CapiGenerator.Parser;

public abstract class BaseParserOutputChannel
{
    public abstract void OnReceiveConstant(ReadOnlySpan<CConstant> constants);
    public void OnReceiveConstant(CConstant constant)
    {
        OnReceiveConstant([constant]);
    }

    public abstract void OnReceiveEnum(ReadOnlySpan<CEnum> enums);
    public void OnReceiveEnum(CEnum enumValue)
    {
        OnReceiveEnum([enumValue]);
    }

    public abstract void OnReceiveStruct(ReadOnlySpan<CStruct> structs);
    public void OnReceiveStruct(CStruct structValue)
    {
        OnReceiveStruct([structValue]);
    }

    public abstract void OnReceiveFunction(ReadOnlySpan<CFunction> types);
    public void OnReceiveFunction(CFunction type)
    {
        OnReceiveFunction([type]);
    }

    public abstract void OnReceiveTypedef(ReadOnlySpan<CTypedef> types);
    public void OnReceiveTypedef(CTypedef type)
    {
        OnReceiveTypedef([type]);
    }
}