using CapiGenerator.ModelFactory;

namespace CapiGenerator;


public class CapiFactory
{
    public ConstModelFactory ConstFactory { get; } = new();


    public CapiModelLookup ToModelLookup()
    {
        var lookup = new CapiModelLookup
        {
            ConstLookup = ConstFactory
        };
        return lookup;
    }

    internal void AddResult(CapiParserResult result)
    {
        result.ConstLookup.TransferOwnership(ConstFactory);
    }
}