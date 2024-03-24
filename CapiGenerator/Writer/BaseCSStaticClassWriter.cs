using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;

public abstract class BaseCSStaticClassWriter
{
    public abstract Task Write(CSStaticClass csStaticClass, CSWriteConfig writeConfig);
}

