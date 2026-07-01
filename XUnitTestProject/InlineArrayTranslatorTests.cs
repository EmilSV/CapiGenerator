using CapiGenerator;
using CapiGenerator.CModel;
using CapiGenerator.CSModel;
using CapiGenerator.Parser;
using CapiGenerator.Translator;
using CppAst;

namespace XUnitTestProject;

public sealed class InlineArrayTranslatorTests
{
    [Fact]
    public void FixedArrayWithBuiltInSizeUsesBuiltInInlineArrayType()
    {
        var csStruct = TranslateSingleStruct("""
            struct Sample {
                int data[16];
            };
            """);

        var field = Assert.Single(csStruct.Fields.ToArray());
        var inlineArrayType = Assert.IsType<CSInlineArrayType>(field.Type.Type);

        Assert.Equal(16u, inlineArrayType.Size);
        Assert.Empty(csStruct.NestedTypes.ToArray());
    }

    [Fact]
    public void FixedArrayAboveBuiltInSizeCreatesNestedInlineArrayStruct()
    {
        var csStruct = TranslateSingleStruct("""
            struct Sample {
                int data[32];
            };
            """);

        var field = Assert.Single(csStruct.Fields.ToArray());
        var nestedInlineArrayStruct = Assert.IsType<CSStruct>(field.Type.Type);
        var nestedType = Assert.Single(csStruct.NestedTypes.ToArray());
        var elementField = Assert.Single(nestedInlineArrayStruct.Fields.ToArray());
        var attribute = Assert.Single(nestedInlineArrayStruct.Attributes);

        Assert.Same(nestedType, nestedInlineArrayStruct);
        Assert.Equal("DataInlineArray32", nestedInlineArrayStruct.Name);
        Assert.Equal(typeof(System.Runtime.CompilerServices.InlineArrayAttribute), attribute.GetAttributeType());
        Assert.Equal(["32"], attribute.CtorArgs.ToArray());
        Assert.Equal(CSAccessModifier.Public, nestedInlineArrayStruct.AccessModifier);
        Assert.Equal("_element0", elementField.Name);
        Assert.IsType<CSPrimitiveType>(elementField.Type.Type);
    }

    [Fact]
    public void MultiDimensionalFixedArrayCanMixBuiltInAndNestedInlineArrayTypes()
    {
        var csStruct = TranslateSingleStruct("""
            struct Sample {
                int values[32][4];
            };
            """);

        var field = Assert.Single(csStruct.Fields.ToArray());
        var outerInlineArrayStruct = Assert.IsType<CSStruct>(field.Type.Type);
        var outerElementField = Assert.Single(outerInlineArrayStruct.Fields.ToArray());
        var innerInlineArrayType = Assert.IsType<CSInlineArrayType>(outerElementField.Type.Type);

        Assert.Equal("ValuesInlineArray32", outerInlineArrayStruct.Name);
        Assert.Equal(4u, innerInlineArrayType.Size);
        Assert.Single(csStruct.NestedTypes.ToArray());
    }

    [Fact]
    public void NestedStructFixedArrayAboveBuiltInSizeIsConverted()
    {
        var csStruct = TranslateSingleStruct("""
            struct Outer {
                struct Inner {
                    int data[32];
                } inner;
            };
            """);

        var innerStruct = Assert.IsType<CSStruct>(Assert.Single(csStruct.NestedTypes.ToArray()));
        var field = Assert.Single(innerStruct.Fields.ToArray());
        var nestedInlineArrayStruct = Assert.IsType<CSStruct>(field.Type.Type);

        Assert.Equal("DataInlineArray32", nestedInlineArrayStruct.Name);
        Assert.Contains(nestedInlineArrayStruct, innerStruct.NestedTypes.ToArray());
    }

    [Fact]
    public void FunctionPointerArrayParameterAboveBuiltInSizeIsConverted()
    {
        var csStruct = TranslateSingleStruct("""
            struct Sample {
                void (*callback)(int data[32]);
            };
            """);

        var field = Assert.Single(csStruct.Fields.ToArray());
        var functionType = Assert.IsType<CSUnmanagedFunctionType>(field.Type.Type);
        var parameterType = Assert.Single(functionType.ParameterTypes.ToArray());
        var nestedInlineArrayStruct = Assert.IsType<CSStruct>(parameterType.Type);

        Assert.Equal("CallbackInlineArray32", nestedInlineArrayStruct.Name);
        Assert.Contains(nestedInlineArrayStruct, csStruct.NestedTypes.ToArray());
        Assert.DoesNotContain(parameterType.Modifiers, modifier => modifier is CSFixedInlineArrayType);
    }

    [Fact]
    public void FunctionPointerPointerToArrayParameterAboveBuiltInSizeIsConverted()
    {
        var csStruct = TranslateSingleStruct("""
            struct Sample {
                void (*callback)(int (*data)[32]);
            };
            """);

        var field = Assert.Single(csStruct.Fields.ToArray());
        var functionType = Assert.IsType<CSUnmanagedFunctionType>(field.Type.Type);
        var parameterType = Assert.Single(functionType.ParameterTypes.ToArray());
        var nestedInlineArrayStruct = Assert.IsType<CSStruct>(parameterType.Type);

        Assert.Equal("CallbackInlineArray32", nestedInlineArrayStruct.Name);
        Assert.Contains(nestedInlineArrayStruct, csStruct.NestedTypes.ToArray());
        Assert.Contains(parameterType.Modifiers, modifier => modifier is CsPointerType);
        Assert.DoesNotContain(parameterType.Modifiers, modifier => modifier is CSFixedInlineArrayType);
        Assert.DoesNotContain(parameterType.ToString(), "FixedInlineArrayType");
    }

    [Fact]
    public void TypedefFunctionPointerPointerToArrayParameterAboveBuiltInSizeIsConverted()
    {
        var csStruct = TranslateSingleTypedefStruct("""
            typedef void (*Callback)(int (*data)[32]);
            """);

        var field = Assert.Single(csStruct.Fields.ToArray());
        var functionType = Assert.IsType<CSUnmanagedFunctionType>(field.Type.Type);
        var parameterType = Assert.Single(functionType.ParameterTypes.ToArray());
        var nestedInlineArrayStruct = Assert.IsType<CSStruct>(parameterType.Type);

        Assert.Equal("ValueInlineArray32", nestedInlineArrayStruct.Name);
        Assert.Contains(nestedInlineArrayStruct, csStruct.NestedTypes.ToArray());
        Assert.Contains(parameterType.Modifiers, modifier => modifier is CsPointerType);
        Assert.DoesNotContain(parameterType.Modifiers, modifier => modifier is CSFixedInlineArrayType);
        Assert.DoesNotContain(parameterType.ToString(), "FixedInlineArrayType");
    }

    [Fact]
    public void TypedefFunctionPointerUnsizedArrayParameterIsConvertedToPointer()
    {
        var csStruct = TranslateSingleTypedefStruct("""
            typedef int (*SDL_AppInit_func)(void **appstate, int argc, char *argv[]);
            """);

        var field = Assert.Single(csStruct.Fields.ToArray());
        var functionType = Assert.IsType<CSUnmanagedFunctionType>(field.Type.Type);
        var parameterTypes = functionType.ParameterTypes.ToArray();
        var argvParameterType = parameterTypes[2];

        Assert.Empty(csStruct.NestedTypes.ToArray());
        Assert.IsType<CSPrimitiveType>(argvParameterType.Type);
        Assert.Equal(2, argvParameterType.Modifiers.Count(modifier => modifier is CsPointerType));
        Assert.DoesNotContain(argvParameterType.Modifiers, modifier => modifier is CSFixedInlineArrayType);
        Assert.DoesNotContain(field.Type.ToString(), "4294967295");
    }

    private static CSStruct TranslateSingleStruct(string header)
    {
        var compilationUnit = ParseHeader(header);
        var translationUnit = new CSTranslationUnit();
        translationUnit.AddTranslator(new CSStructTranslator());
        translationUnit.Translate([compilationUnit]);
        return Assert.Single(translationUnit.GetCSStructsEnumerable());
    }

    private static CSStruct TranslateSingleTypedefStruct(string header)
    {
        var compilationUnit = ParseHeader(header, includeTypedefParser: true);
        var translationUnit = new CSTranslationUnit();
        translationUnit.AddTranslator(new CSTypedefTranslator());
        translationUnit.Translate([compilationUnit]);
        return Assert.Single(translationUnit.GetCSStructsEnumerable());
    }

    private static CCompilationUnit ParseHeader(string header, bool includeTypedefParser = false)
    {
        var headerPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.h");
        File.WriteAllText(headerPath, header);

        try
        {
            var fakeCStdHeaderPath = FakeCStdHeader.CreateFakeStdHeaderFolder();
            var options = new CppParserOptions();
            options.IncludeFolders.Add(fakeCStdHeaderPath);

            var cppCompilation = CppParser.ParseFile(headerPath, options);
            Assert.False(
                cppCompilation.HasErrors,
                string.Join(Environment.NewLine, cppCompilation.Diagnostics.Messages.Select(message => message.ToString())));

            var compilationUnit = new CCompilationUnit();
            compilationUnit.AddParser(includeTypedefParser
                ? [new TypedefParser()]
                :
                [
                    new StructParser(),
                    new UnionParser(),
                ]);
            compilationUnit.Parse([cppCompilation]);

            return compilationUnit;
        }
        finally
        {
            File.Delete(headerPath);
        }
    }
}
