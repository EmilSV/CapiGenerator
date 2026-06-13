
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseCSCallableType : BaseCSAstItem
{
    public readonly ChildList<CSParameter, BaseCSCallableType> Parameters;
    public BaseCSCallableType()
    {
        Parameters = new ChildList<CSParameter, BaseCSCallableType>(this);
    }
}
