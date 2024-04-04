using CapiGenerator.CSModel;

namespace CapiGenerator.Translator;

public static class BuildInTranslation
{
    public static void AddTranslation(CSTranslationUnit translationUnit)
    {
        translationUnit.AddPredefinedTranslation("ptrdiff_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.NInt));
        translationUnit.AddPredefinedTranslation("size_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.NUInt));
        translationUnit.AddPredefinedTranslation("int8_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.SByte));
        translationUnit.AddPredefinedTranslation("uint8_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.Byte));
        translationUnit.AddPredefinedTranslation("int16_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.Short));
        translationUnit.AddPredefinedTranslation("uint16_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.UShort));
        translationUnit.AddPredefinedTranslation("int32_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.Int));
        translationUnit.AddPredefinedTranslation("uint32_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.UInt));
        translationUnit.AddPredefinedTranslation("int64_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.Long));
        translationUnit.AddPredefinedTranslation("uint64_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.ULong));
        translationUnit.AddPredefinedTranslation("intptr_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.NInt));
        translationUnit.AddPredefinedTranslation("uintptr_t", CSPrimitiveType.Get(CSPrimitiveType.Kind.NUInt));
        
    }
}