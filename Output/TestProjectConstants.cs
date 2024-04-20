using System;
namespace TestProject;
public static class TestProjectConstants
{
    public const int SIMPLE_EXPRESSION = 1 + 2;
    public const int COMPLEX_EXPRESSION_REF = (1 + 2) * TestProject.TestProjectConstants.SIMPLE_EXPRESSION;
}
