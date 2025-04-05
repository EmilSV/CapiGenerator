using System.Text.RegularExpressions;

namespace CapiGenerator.Extensions;

public static partial class NewLineSplitExtension
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
}