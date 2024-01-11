using CapiGenerator.Model;

namespace CapiGenerator.Parser;

public abstract class BaseParserInputChannel
{
    public abstract ReadOnlySpan<CConstant> GetConstants();
    public abstract ReadOnlySpan<CEnum> GetEnums();
    public abstract ReadOnlySpan<CStruct> GetStructs();
    public abstract ReadOnlySpan<ICType> GetTypes();
}