using CapiGenerator.Model;

namespace CapiGenerator.Parser;

public abstract class BaseParserInputChannel
{
    public abstract ReadOnlySpan<CConst> GetConstants();
}