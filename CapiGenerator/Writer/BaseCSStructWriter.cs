using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;

public abstract class BaseCSStructWriter
{
    public abstract Task Write(CSStruct csStruct, CSWriteConfig writeConfig);
}

