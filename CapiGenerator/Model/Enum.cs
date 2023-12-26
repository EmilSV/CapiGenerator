namespace CapiGenerator.Model;


public class Enum
{

    internal Enum(
        ModelRef<Enum> modelRef,
        BaseModelRefLookup<Enum> owingFactory,
        EnumInput input,
        EnumOutput output)
    {
        ModelRef = modelRef;
        OwingFactory = owingFactory;
        _input = input;
        _output = output;
    }
}