using CapiGenerator;
using CapiGenerator.ConstantToken;
using CapiGenerator.Model;
using CppAst;


namespace XUnitTestProject
{
    public class ConstTest
    {
        [Fact]
        public void SimpleLiteralTest1()
        {
            const string TestContent = """
                #define LITERAL 3            
            """;

            var cppCompilation = CppParser.Parse(TestContent);

            Assert.NotNull(cppCompilation);
            Assert.False(cppCompilation.HasErrors);

            var generator = new CapiGeneratorUnit();

            var @namespace = "MyTestNamespace";
            generator.AddCompilation(cppCompilation, @namespace, "MyTestFolder");

            var newConstsCreated = generator.GetConstants();

            Assert.Single(newConstsCreated);

            var newConst = newConstsCreated[0];

            Assert.Equal("LITERAL", newConst.InputName);

            var tokens = newConst.Output.Tokens;

            Assert.Single(tokens);

            Assert.True(tokens[0] is ConstantLiteralToken token && token.Value == "3");
        }

        [Fact]
        public void ComplexExpresionRef()
        {
            const string TestContent = """
                #define SIMPLE_EXPRESSION 1 + 2

                #define COMPLEX_EXPRESION_REF (1 + 2) * SIMPLE_EXPRESSION      
            """;

            var cppCompilation = CppParser.Parse(TestContent);

            Assert.NotNull(cppCompilation);
            Assert.False(cppCompilation.HasErrors);

            var generator = new CapiGeneratorUnit();

            var @namespace = "MyTestNamespace";
            generator.AddCompilation(cppCompilation, @namespace, "MyTestFolder");

            var newConstsCreated = generator.GetConstants();

            Assert.Equal(2, newConstsCreated.Count);

            Dictionary<string, Constant> nameToConst = new();

            foreach (var item in newConstsCreated)
            {
                nameToConst.Add(item.InputName, item);
            }

            var simpleExpresion = nameToConst["SIMPLE_EXPRESSION"];

            Assert.Equal(new BaseConstantToken[] {
                new ConstantLiteralToken("1"),
                new ConstantPunctuationToken() {Type = PunctuationType.Plus } ,
                new ConstantLiteralToken("2"),
            },
            simpleExpresion.Output.Tokens);

            var ComplexExpresionRef = nameToConst["COMPLEX_EXPRESION_REF"];

            Assert.Equal(new BaseConstantToken[] {
                new ConstantPunctuationToken() {Type = PunctuationType.LeftParenthesis } ,
                new ConstantLiteralToken("1"),
                new ConstantPunctuationToken() {Type = PunctuationType.Plus } ,
                new ConstantLiteralToken("2"),
                new ConstantPunctuationToken() {Type = PunctuationType.RightParenthesis },
                new ConstantPunctuationToken() {Type = PunctuationType.Multiply },
                new ConstIdentifierToken() { Value = simpleExpresion}

            }, ComplexExpresionRef.Output.Tokens);
        }
    }
}