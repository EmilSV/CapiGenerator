using CapiGenerator;
using CapiGenerator.Parser;
using CppAst;

namespace XUnitTestProject;

public sealed class FunctionParserTests
{
    [Fact]
    public void InlineFunctionsAreNotParsedBecauseTheyAreNotExported()
    {
        var compilationUnit = ParseFunctions("""
            extern int exported_function(int x);
            static inline int static_inline_function(int x) { return x; }
            __attribute__((always_inline)) static __inline__ int gnu_force_inline_function(int x) { return x; }
            #ifdef _MSC_VER
            __forceinline int msvc_force_inline_function(int x) { return x; }
            #endif
            """);

        Assert.NotNull(compilationUnit.GetFunctionByName("exported_function"));
        Assert.Null(compilationUnit.GetFunctionByName("static_inline_function"));
        Assert.Null(compilationUnit.GetFunctionByName("gnu_force_inline_function"));
        Assert.Null(compilationUnit.GetFunctionByName("msvc_force_inline_function"));
    }

    [Fact]
    public void ExternInlineFunctionIsParsedBecauseItIsExported()
    {
        var compilationUnit = ParseFunctions("""
            extern inline int extern_inline_function(int x) { return x; }
            """);

        Assert.NotNull(compilationUnit.GetFunctionByName("extern_inline_function"));
    }

    [Fact]
    public void VariadicFunctionsAreNotParsed()
    {
        var compilationUnit = ParseFunctions("""
            extern int fixed_function(const char *fmt);
            extern int variadic_function(const char *fmt, ...);
            """);

        Assert.NotNull(compilationUnit.GetFunctionByName("fixed_function"));
        Assert.Null(compilationUnit.GetFunctionByName("variadic_function"));
    }

    private static CCompilationUnit ParseFunctions(string header)
    {
        var headerPath = Path.Combine(Path.GetTempPath(), $"functions-{Guid.NewGuid():N}.h");

        try
        {
            File.WriteAllText(headerPath, header);

            var options = new CppParserOptions();
            options.IncludeFolders.Add(FakeCStdHeader.CreateFakeStdHeaderFolder());

            var cppCompilation = CppParser.ParseFile(headerPath, options);
            Assert.False(
                cppCompilation.HasErrors,
                string.Join(Environment.NewLine, cppCompilation.Diagnostics.Messages.Select(message => message.ToString())));

            var compilationUnit = new CCompilationUnit();
            compilationUnit.AddParser(new FunctionParser());
            compilationUnit.Parse([cppCompilation]);
            return compilationUnit;
        }
        finally
        {
            File.Delete(headerPath);
        }
    }
}
