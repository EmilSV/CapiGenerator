using CapiGenerator.CSModel;

namespace CapiGenerator.Parser;

public abstract class BaseTranslatorInputChannel
{
    public abstract ReadOnlySpan<CSEnum> GetEnums();
    public abstract ReadOnlySpan<CSStruct> GetStructs();
    public abstract ReadOnlySpan<CSStaticClass> GetStaticClass();
}