using System;
namespace TestProject;
public static class TestProjectConstants
{
    public const int LITERAL = 3;
    public const int SIMPLE_EXPRESSION = 1 + 2;
    public const int SIMPLE_EXPRESSION_WITH_PARENTHESIS = (1 + 2);
    public const int SIMPLE_EXPRESSION_WITH_PARENTHESIS_AND_UNARY = -(1 + 2);
    public const int COMPLEX_EXPRESION = (1 + 2) * 3 << 1;
    public const int COMPLEX_EXPRESION_REF = (1 + 2) * ;
    public static System.ReadonlySpan<byte> TEXT
    {
        get => "Hello World"u8;
    }
    public static System.ReadonlySpan<byte> TEXT_WITH_ESCAPE
    {
        get => "Hello \"World\""u8;
    }
    public const float FLOAT_EXPRESION = (1.3 + 2.6);
}
