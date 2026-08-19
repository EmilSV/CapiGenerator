using System.Text;
using System.Text.RegularExpressions;

namespace CapiGenerator.Extensions;

public static partial class StringExtensions
{
    [GeneratedRegex(@"(\r\n|\r|\n)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    private static partial Regex GetNewLineRegex();

    public static string[] SplitNewLine(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return [];
        }

        return GetNewLineRegex().Split(str).Where(x => !GetNewLineRegex().IsMatch(x)).ToArray();
    }

    public static string ToPascalCaseIdentifier(this string name)
    {
        var builder = new StringBuilder();
        var capitalizeNext = true;

        foreach (var character in name)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(capitalizeNext ? char.ToUpperInvariant(character) : character);
                capitalizeNext = false;
            }
            else
            {
                capitalizeNext = true;
            }
        }

        return builder.ToString();
    }
}
