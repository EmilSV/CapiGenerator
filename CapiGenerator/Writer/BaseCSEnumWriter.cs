using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;

public abstract class BaseCSEnumWriter
{
    public abstract Task Write(CSEnum csEnum, CSWriteConfig writeConfig);
}

