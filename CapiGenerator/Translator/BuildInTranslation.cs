using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;

namespace CapiGenerator.Translator;

public static class BuildInTranslation
{
    public static void AddTranslation(CSTranslationUnit translationUnit)
    {
        translationUnit
        .AddPredefinedTranslation(CPrimitiveType.Instances.Int8_t, CSPrimitiveType.Instances.SByte)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Int16_t, CSPrimitiveType.Instances.Short)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Int32_t, CSPrimitiveType.Instances.Int)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Int64_t, CSPrimitiveType.Instances.Long)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UInt8_t, CSPrimitiveType.Instances.Byte)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UInt16_t, CSPrimitiveType.Instances.UShort)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UInt32_t, CSPrimitiveType.Instances.UInt)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UInt64_t, CSPrimitiveType.Instances.ULong)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Intptr_t, CSPrimitiveType.Instances.NInt)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UIntptr_t, CSPrimitiveType.Instances.NUInt)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Size_t, CSPrimitiveType.Instances.NUInt)
        .AddPredefinedTranslation(CPrimitiveType.Instances.SSize_t, CSPrimitiveType.Instances.NInt)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Ptrdiff_t, CSPrimitiveType.Instances.NInt)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Char, CSPrimitiveType.Instances.Bool)
        .AddPredefinedTranslation(CPrimitiveType.Instances.SignedChar, CSPrimitiveType.Instances.SByte)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UnsignedChar, CSPrimitiveType.Instances.Char)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Short, CSPrimitiveType.Instances.Short)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UnsignedShortInt, CSPrimitiveType.Instances.UShort)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Int, CSPrimitiveType.Instances.Int)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UnsignedInt, CSPrimitiveType.Instances.UInt)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Long, CSPrimitiveType.Instances.Int)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UnsignedLong, CSPrimitiveType.Instances.UInt)
        .AddPredefinedTranslation(CPrimitiveType.Instances.LongLong, CSPrimitiveType.Instances.Long)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UnsignedLongLong, CSPrimitiveType.Instances.ULong)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Float, CSPrimitiveType.Instances.Float)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Double, CSPrimitiveType.Instances.Double)
        .AddPredefinedTranslation(CPrimitiveType.Instances._Bool, CSPrimitiveType.Instances.Bool)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Bool, CSPrimitiveType.Instances.Bool)
        .AddPredefinedTranslation(CPrimitiveType.Instances.CString, CSUft8LiteralType.Instance)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Void, CSPrimitiveType.Instances.Void);


        translationUnit
            .BanType(CPrimitiveType.Instances.LongDouble);

    }
}