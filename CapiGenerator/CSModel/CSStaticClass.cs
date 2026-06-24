namespace CapiGenerator.CSModel;

public class CSStaticClass : BaseCSMemberContainer
{
    public CSAccessModifier AccessModifier = CSAccessModifier.Public;
    public bool IsUnsafe = true;
    public bool IsPartial;
}
