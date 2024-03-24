using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;

public abstract class BaseCSWriter
{
    public abstract Task Write(CSEnum csEnum, CSWriteConfig writeConfig);
    public abstract Task Write(CSStruct csStruct, CSWriteConfig writeConfig);
    public abstract Task Write(CSStaticClass csStaticClass, CSWriteConfig writeConfig);
}