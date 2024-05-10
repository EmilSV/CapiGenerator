using CapiGenerator.CModel;

namespace CapiGenerator.Parser;

public abstract class BaseParserInputChannel
{
    public abstract ReadOnlySpan<BaseCConstant> GetConstants();
    public abstract ReadOnlySpan<CEnum> GetEnums();
    public abstract ReadOnlySpan<CStruct> GetStructs();
    public abstract ReadOnlySpan<CFunction> GetFunctions();
    public abstract ReadOnlySpan<CTypedef> GetTypedefs();
}