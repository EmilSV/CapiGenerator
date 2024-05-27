using CapiGenerator.CModel.BuiltinTypedefs;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;

namespace CapiGenerator.Translator;

public static class BuildInTranslation
{
    public static void AddTranslation(CSTranslationUnit translationUnit)
    {
        translationUnit
        .AddPredefinedTranslation(CPrimitiveType.Instances.Char, CSPrimitiveType.Instances.Byte)
        .AddPredefinedTranslation(CPrimitiveType.Instances.SignedChar, CSPrimitiveType.Instances.SByte)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UnsignedChar, CSPrimitiveType.Instances.Byte)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Short, CSPrimitiveType.Instances.Short)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UnsignedShortInt, CSPrimitiveType.Instances.UShort)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Int, CSPrimitiveType.Instances.Int)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UnsignedInt, CSPrimitiveType.Instances.UInt)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Long, CSCLongType.Instance)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UnsignedLong, CSCULongType.Instance)
        .AddPredefinedTranslation(CPrimitiveType.Instances.LongLong, CSPrimitiveType.Instances.Long)
        .AddPredefinedTranslation(CPrimitiveType.Instances.UnsignedLongLong, CSPrimitiveType.Instances.ULong)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Float, CSPrimitiveType.Instances.Float)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Double, CSPrimitiveType.Instances.Double)
        .AddPredefinedTranslation(CPrimitiveType.Instances._Bool, CSPrimitiveType.Instances.Bool)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Bool, CSPrimitiveType.Instances.Bool)
        .AddPredefinedTranslation(CPrimitiveType.Instances.CString, CSUft8LiteralType.Instance)
        .AddPredefinedTranslation(CPrimitiveType.Instances.Void, CSPrimitiveType.Instances.Void)
        .AddPredefinedTranslation(Int8TBuiltinTypedef.Instance, CSPrimitiveType.Instances.SByte)
        .AddPredefinedTranslation(Int16TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Short)
        .AddPredefinedTranslation(Int32TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Int)
        .AddPredefinedTranslation(Int64TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Long)
        .AddPredefinedTranslation(IntFast8TBuiltinTypedef.Instance, CSPrimitiveType.Instances.SByte)
        .AddPredefinedTranslation(IntFast16TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Int)
        .AddPredefinedTranslation(IntFast32TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Int)
        .AddPredefinedTranslation(IntFast64TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Long)
        .AddPredefinedTranslation(IntLeast8TBuiltinTypedef.Instance, CSPrimitiveType.Instances.SByte)
        .AddPredefinedTranslation(IntLeast16TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Short)
        .AddPredefinedTranslation(IntLeast32TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Int)
        .AddPredefinedTranslation(IntLeast64TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Long)
        .AddPredefinedTranslation(IntMaxTBuiltinTypedef.Instance, CSPrimitiveType.Instances.Long)
        .AddPredefinedTranslation(UInt8TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Byte)
        .AddPredefinedTranslation(UInt16TBuiltinTypedef.Instance, CSPrimitiveType.Instances.UShort)
        .AddPredefinedTranslation(UInt32TBuiltinTypedef.Instance, CSPrimitiveType.Instances.UInt)
        .AddPredefinedTranslation(UInt64TBuiltinTypedef.Instance, CSPrimitiveType.Instances.ULong)
        .AddPredefinedTranslation(UIntFast8TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Byte)
        .AddPredefinedTranslation(UIntFast16TBuiltinTypedef.Instance, CSPrimitiveType.Instances.UInt)
        .AddPredefinedTranslation(UIntFast32TBuiltinTypedef.Instance, CSPrimitiveType.Instances.UInt)
        .AddPredefinedTranslation(UIntFast64TBuiltinTypedef.Instance, CSPrimitiveType.Instances.ULong)
        .AddPredefinedTranslation(UIntLeast8TBuiltinTypedef.Instance, CSPrimitiveType.Instances.Byte)
        .AddPredefinedTranslation(UIntLeast16TBuiltinTypedef.Instance, CSPrimitiveType.Instances.UShort)
        .AddPredefinedTranslation(UIntLeast32TBuiltinTypedef.Instance, CSPrimitiveType.Instances.UInt)
        .AddPredefinedTranslation(UIntLeast64TBuiltinTypedef.Instance, CSPrimitiveType.Instances.ULong)
        .AddPredefinedTranslation(UIntMaxTBuiltinTypedef.Instance, CSPrimitiveType.Instances.ULong)
        .AddPredefinedTranslation(IntPtrTBuiltinTypedef.Instance, CSPrimitiveType.Instances.NInt)
        .AddPredefinedTranslation(UIntPtrTBuiltinTypedef.Instance, CSPrimitiveType.Instances.NUInt)
        .AddPredefinedTranslation(SizeTBuiltinTypedefs.Instance, CSPrimitiveType.Instances.NUInt)
        .AddPredefinedTranslation(PtrdiffTBuiltinTypedefs.Instance, CSPrimitiveType.Instances.NInt);

        translationUnit
            .BanType(CPrimitiveType.Instances.LongDouble);

    }
}