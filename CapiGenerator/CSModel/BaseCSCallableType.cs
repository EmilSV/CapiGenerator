
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseCSCallableType : BaseCSAstItem
{
    public readonly ChildList<CSParameter, BaseCSCallableType> Parameters;

    protected BaseCSCallableType()
    {
        Parameters = new ChildList<CSParameter, BaseCSCallableType>(this);
    }

    protected BaseCSCallableType(object primarySource)
        : base(primarySource)
    {
        Parameters = new ChildList<CSParameter, BaseCSCallableType>(this);
    }
}
