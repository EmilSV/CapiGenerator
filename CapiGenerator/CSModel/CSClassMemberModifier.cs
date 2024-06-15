namespace CapiGenerator.CSModel;

[System.Flags]
public enum CSClassMemberModifier
{
    None = 0,
    Private = 1 << 0,
    Public = 1 << 1,
    Internal = 1 << 2,
    Protected = 1 << 3,
    ProtectedInternal = Protected | Internal,
    PrivateProtected = Private | Protected,
    Static = 1 << 4,
    ReadOnly = 1 << 5,
    Virtual = 1 << 6,
    Override = 1 << 7,
    Sealed = 1 << 8,
    Const = 1 << 9,
    Partial = 1 << 10,
    Async = 1 << 11,
    Unsafe = 1 << 12,
    Explicit = 1 << 13,
    Implicit = 1 << 14,
    Extern = 1 << 15,
    Abstract = 1 << 16,
    New = 1 << 17,
    Operator = 1 << 18,
    Volatile = 1 << 19,
    Event = 1 << 20,
    Fixed = 1 << 21,
}

public static class CSClassMemberModifierConsts
{
    public const CSClassMemberModifier PRIVATE = CSClassMemberModifier.Private;
    public const CSClassMemberModifier PUBLIC = CSClassMemberModifier.Public;
    public const CSClassMemberModifier INTERNAL = CSClassMemberModifier.Internal;
    public const CSClassMemberModifier PROTECTED = CSClassMemberModifier.Protected;
    public const CSClassMemberModifier PROTECTED_INTERNAL = CSClassMemberModifier.ProtectedInternal;
    public const CSClassMemberModifier PRIVATE_PROTECTED = CSClassMemberModifier.PrivateProtected;
    public const CSClassMemberModifier STATIC = CSClassMemberModifier.Static;
    public const CSClassMemberModifier READONLY = CSClassMemberModifier.ReadOnly;
    public const CSClassMemberModifier VIRTUAL = CSClassMemberModifier.Virtual;
    public const CSClassMemberModifier OVERRIDE = CSClassMemberModifier.Override;
    public const CSClassMemberModifier SEALED = CSClassMemberModifier.Sealed;
    public const CSClassMemberModifier CONST = CSClassMemberModifier.Const;
    public const CSClassMemberModifier PARTIAL = CSClassMemberModifier.Partial;
    public const CSClassMemberModifier ASYNC = CSClassMemberModifier.Async;
    public const CSClassMemberModifier UNSAFE = CSClassMemberModifier.Unsafe;
    public const CSClassMemberModifier EXPLICIT = CSClassMemberModifier.Explicit;
    public const CSClassMemberModifier IMPLICIT = CSClassMemberModifier.Implicit;
    public const CSClassMemberModifier EXTERN = CSClassMemberModifier.Extern;
    public const CSClassMemberModifier ABSTRACT = CSClassMemberModifier.Abstract;
    public const CSClassMemberModifier NEW = CSClassMemberModifier.New;
    public const CSClassMemberModifier OPERATOR = CSClassMemberModifier.Operator;
    public const CSClassMemberModifier VOLATILE = CSClassMemberModifier.Volatile;
    public const CSClassMemberModifier EVENT = CSClassMemberModifier.Event;
    public const CSClassMemberModifier FIXED = CSClassMemberModifier.Fixed;
}