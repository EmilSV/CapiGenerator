public static class ConstTestConsts
{
    public const long LITERAL = 3;
    public const long SIMPLE_EXPRESSION = 1 + 2;
    public const long SIMPLE_EXPRESSION_WITH_PARENTHESIS = (1 + 2);
    public const long SIMPLE_EXPRESSION_WITH_PARENTHESIS_AND_UNARY = -(1 + 2);
    public const double COMPLEX_EXPRESION = (1 + 2) * 3 << 1;
    public const long COMPLEX_EXPRESION_REF = (1 + 2) * SIMPLE_EXPRESSION;
    public static System.ReadOnlySpan<byte> TEXT => "Hello World"u8;
}
