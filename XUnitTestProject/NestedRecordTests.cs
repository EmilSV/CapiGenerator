using CapiGenerator;
using CapiGenerator.CModel;
using CapiGenerator.Parser;
using CppAst;
using Xunit;

namespace XUnitTestProject;

public sealed class NestedRecordTests
{
    [Fact]
    public void NamedNestedStructIsStoredInParentNestedTypes()
    {
        var compilationUnit = ParseHeader("""
            struct Outer {
                struct Inner {
                    int value;
                } inner;
            };
            """);

        var outer = Assert.Single(compilationUnit.GetStructEnumerable());
        var nested = Assert.IsType<CStruct>(Assert.Single(outer.NestedTypes.ToArray()));
        var field = Assert.Single(outer.Fields.ToArray());

        Assert.Equal("Outer", outer.Name);
        Assert.Equal("Inner", nested.Name);
        Assert.False(nested.IsAnonymous);
        Assert.Same(nested, field.GetFieldType().GetCType());
    }

    [Fact]
    public void MultiLevelNestedStructsAreStoredOnTheirDirectParent()
    {
        var compilationUnit = ParseHeader("""
            struct Outer {
                struct Inner {
                    struct Deep {
                        int value;
                    } deep;
                } inner;
            };
            """);

        var outer = Assert.Single(compilationUnit.GetStructEnumerable());
        var inner = Assert.IsType<CStruct>(Assert.Single(outer.NestedTypes.ToArray()));
        var deep = Assert.IsType<CStruct>(Assert.Single(inner.NestedTypes.ToArray()));
        var outerField = Assert.Single(outer.Fields.ToArray());
        var innerField = Assert.Single(inner.Fields.ToArray());

        Assert.Equal("Inner", inner.Name);
        Assert.Equal("Deep", deep.Name);
        Assert.Same(inner, outerField.GetFieldType().GetCType());
        Assert.Same(deep, innerField.GetFieldType().GetCType());
    }

    [Fact]
    public void AnonymousNestedStructIsStoredInParentNestedTypes()
    {
        var compilationUnit = ParseHeader("""
            struct Outer {
                struct {
                    int value;
                } anonymousField;
            };
            """);

        var outer = Assert.Single(compilationUnit.GetStructEnumerable());
        var nested = Assert.IsType<CStruct>(Assert.Single(outer.NestedTypes.ToArray()));
        var field = Assert.Single(outer.Fields.ToArray());

        Assert.Equal("AnonymousField", nested.Name);
        Assert.True(nested.IsAnonymous);
        Assert.Same(nested, field.GetFieldType().GetCType());
    }

    private static CCompilationUnit ParseHeader(string header)
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
            compilationUnit.AddParser([
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
