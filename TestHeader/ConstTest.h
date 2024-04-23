// #define LITERAL 3

// #define SIMPLE_EXPRESSION 1 + 2

// #define SIMPLE_EXPRESSION_WITH_PARENTHESIS (1 + 2)

// #define SIMPLE_EXPRESSION_WITH_PARENTHESIS_AND_UNARY -(1 + 2)

// #define COMPLEX_EXPRESSION (1 + 2) * 3 << 1

// #define COMPLEX_EXPRESSION_REF (1 + 2) * SIMPLE_EXPRESSION

// #define EMPTY_DEFINE
enum SimpleEnum
{
    SimpleEnum_EnumValue1,
    SimpleEnum_EnumValue2,
    SimpleEnum_EnumValue3
};

#define SIMPLE_EXPRESSION SimpleEnum_EnumValue1 + 1
