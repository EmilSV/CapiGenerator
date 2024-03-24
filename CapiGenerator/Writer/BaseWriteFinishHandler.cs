using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;

public abstract class BaseWriteFinishHandler
{
    public abstract void OnFinish(CSStruct csStruct);
    public abstract void OnFinish(CSEnum csEnum);
    public abstract void OnFinish(CSStaticClass csStaticClass);
}