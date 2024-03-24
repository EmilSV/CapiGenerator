using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;


public class CSWriter(
    BaseCSStructWriter csStructWriter,
    BaseCSEnumWriter csEnumWriter,
    BaseCSStaticClassWriter csStaticClassWriter
) : BaseCSWriter
{
    public override async Task Write(CSStruct csStruct, CSWriteConfig writeConfig)
    {
        await csStructWriter.Write(csStruct, writeConfig);
    }

    public override async Task Write(CSEnum csEnum, CSWriteConfig writeConfig)
    {
        await csEnumWriter.Write(csEnum, writeConfig);
    }

    public override async Task Write(CSStaticClass csStaticClass, CSWriteConfig writeConfig)
    {
        await csStaticClassWriter.Write(csStaticClass, writeConfig);
    }
}