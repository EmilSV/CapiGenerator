using CapiGenerator.CSModel;

namespace CapiGenerator.Translator;

public abstract class BaseTranslatorOutputChannel
{
    public abstract void OnReceiveStruct(ReadOnlySpan<CSStruct> structs);
    public virtual void OnReceiveStruct(CSStruct structValue)
    {
        OnReceiveStruct([structValue]);
    }
}