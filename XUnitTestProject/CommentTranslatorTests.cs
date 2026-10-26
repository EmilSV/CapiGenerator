using CapiGenerator;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.Parser;
using CppAst;

namespace XUnitTestProject;

public sealed class CommentTranslatorTests
{
    [Fact]
    public void AngleBracketsAndAmpersandsAreEscaped()
    {
        var comment = TranslateFunctionComment("""
            /**
             * Scale < 1 is "zoom out". Scale > 1 is "zoom in". A & B.
             */
            extern void function(void);
            """);

        Assert.Equal("Scale &lt; 1 is \"zoom out\". Scale &gt; 1 is \"zoom in\". A &amp; B.", comment.Summary?.Description);
    }

    [Fact]
    public void TextSplitAtTagLikeAngleBracketsIsRejoinedWithoutSpaces()
    {
        var comment = TranslateFunctionComment("""
            /**
             * Can be cast to an `id<MTLRenderCommandEncoder>`.
             *
             * \returns an `id<MTLRenderCommandEncoder>` on success.
             */
            extern void *function(void);
            """);

        Assert.Equal("Can be cast to an `id&lt;MTLRenderCommandEncoder&gt;`.", comment.Summary?.Description);
        Assert.Equal("an `id&lt;MTLRenderCommandEncoder&gt;` on success.", comment.Return?.Description);
    }

    private static DocComment TranslateFunctionComment(string header)
    {
        var headerPath = Path.Combine(Path.GetTempPath(), $"comments-{Guid.NewGuid():N}.h");

        try
        {
            File.WriteAllText(headerPath, header);

            var options = new CppParserOptions
            {
                ParseComments = true,
            };
            options.IncludeFolders.Add(FakeCStdHeader.CreateFakeStdHeaderFolder());

            var cppCompilation = CppParser.ParseFile(headerPath, options);
            Assert.False(
                cppCompilation.HasErrors,
                string.Join(Environment.NewLine, cppCompilation.Diagnostics.Messages.Select(message => message.ToString())));

            var compilationUnit = new CCompilationUnit();
            compilationUnit.AddParser(new FunctionParser());
            compilationUnit.Parse([cppCompilation]);

            var function = compilationUnit.GetFunctionByName("function");
            Assert.NotNull(function);
            var comment = CCommentTranslator.Translate(function.Comment, compilationUnit);
            Assert.NotNull(comment);
            return comment;
        }
        finally
        {
            File.Delete(headerPath);
        }
    }
}
