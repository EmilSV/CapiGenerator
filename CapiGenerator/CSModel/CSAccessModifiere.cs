namespace CapiGenerator.CSModel;

public enum CSAccessModifier
{
    Public,
    Private,
    Protected,
    Internal,
    ProtectedInternal,
    PrivateProtected
}


public static class CSAccessModifierHelper
{
    public static CSAccessModifier GetAccessModifier(CSClassMemberModifier modifiers)
    {
        if ((modifiers & CSClassMemberModifier.ProtectedInternal) != 0)
        {
            return CSAccessModifier.ProtectedInternal;
        }

        if ((modifiers & CSClassMemberModifier.PrivateProtected) != 0)
        {
            return CSAccessModifier.PrivateProtected;
        }

        if ((modifiers & CSClassMemberModifier.Public) != 0)
        {
            return CSAccessModifier.Public;
        }

        if ((modifiers & CSClassMemberModifier.Private) != 0)
        {
            return CSAccessModifier.Private;
        }

        if ((modifiers & CSClassMemberModifier.Protected) != 0)
        {
            return CSAccessModifier.Protected;
        }

        if ((modifiers & CSClassMemberModifier.Internal) != 0)
        {
            return CSAccessModifier.Internal;
        }

        return CSAccessModifier.Private;
    }
}