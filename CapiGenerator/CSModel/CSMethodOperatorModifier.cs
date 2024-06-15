namespace CapiGenerator.CSModel;

public enum CSMethodOperatorModifier
{
    None = 0,
    Explicit = 1,
    Implicit = 2,
    Operator = 3,
}


public static class CSMethodOperatorModifierHelper
{
    public static CSMethodOperatorModifier GetOperatorModifier(CSClassMemberModifier modifiers)
    {
        if ((modifiers & CSClassMemberModifier.Explicit) != 0)
        {
            return CSMethodOperatorModifier.Explicit;
        }

        if ((modifiers & CSClassMemberModifier.Implicit) != 0)
        {
            return CSMethodOperatorModifier.Implicit;
        }

        if ((modifiers & CSClassMemberModifier.Operator) != 0)
        {
            return CSMethodOperatorModifier.Operator;
        }

        return CSMethodOperatorModifier.None;
    }
}