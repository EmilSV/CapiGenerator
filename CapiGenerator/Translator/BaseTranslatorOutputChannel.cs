using CapiGenerator.CSModel;

namespace CapiGenerator.Translator;

public abstract class BaseTranslatorOutputChannel
{
    public abstract void OnReceiveStruct(ReadOnlySpan<CSStruct> structs);
    public virtual void OnReceiveStruct(CSStruct structValue)
    {
        OnReceiveStruct([structValue]);
    }
    public abstract void OnReceiveEnum(ReadOnlySpan<CSEnum> enums);
    public virtual void OnReceiveEnum(CSEnum enumValue)
    {
        OnReceiveEnum([enumValue]);
    }


    public abstract void OnReceiveStaticClass(ReadOnlySpan<CSStaticClass> staticClasses);
    public virtual void OnReceiveStaticClass(CSStaticClass staticClass)
    {
        OnReceiveStaticClass([staticClass]);
    }
}