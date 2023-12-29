using CapiGenerator.Model;

namespace CapiGenerator.Parser;

public abstract class BaseParserOutputChannel
{
    public abstract void OnReceiveConstant(ReadOnlySpan<CConst> constants);
    public void OnReceiveConstant(CConst constant)
    {
        OnReceiveConstant([constant]);
    }
}