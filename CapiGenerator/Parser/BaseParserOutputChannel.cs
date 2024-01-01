using CapiGenerator.Model;

namespace CapiGenerator.Parser;

public abstract class BaseParserOutputChannel
{
    public abstract void OnReceiveConstant(ReadOnlySpan<CConstant> constants);
    public void OnReceiveConstant(CConstant constant)
    {
        OnReceiveConstant([constant]);
    }
}