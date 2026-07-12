using System.Text;
using CapiGenerator.CModel;
using CapiGenerator.CModel.Comments;
using CapiGenerator.Parser;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.Comments;

public static class CCommentTranslator
{
    public static DocComment? Translate(CBaseComment? comment, CCompilationUnit? compilationUnit = null)
    {
        if (comment is null)
        {
            return null;
        }

        var result = new DocComment();
        Visit(comment, result, compilationUnit);
        return result.HasValue() ? result : null;
    }

    private static void Visit(CBaseComment comment, DocComment result, CCompilationUnit? compilationUnit)
    {
        switch (comment)
        {
            case CFullComment:
                foreach (var child in comment.Children)
                {
                    Visit(child, result, compilationUnit);
                }
                break;

            case CParamCommandComment parameter:
                AddParameter(result, parameter.ParamName, GetText(parameter));
                break;

            case CBlockCommandComment block:
                AddBlock(result, block.CommandName, GetText(block, block.Arguments), compilationUnit);
                break;

            case CParagraphComment paragraph:
                AddParagraph(result, GetText(paragraph));
                break;

            case CVerbatimBlockCommandComment verbatimBlock:
                AddRemarks(result, GetText(verbatimBlock, verbatimBlock.Arguments));
                break;

            case CTemplateParamCommandComment templateParameter:
                AddParameter(result, templateParameter.ParamName, GetText(templateParameter, templateParameter.Arguments));
                break;

            default:
                AddParagraph(result, GetText(comment));
                break;
        }
    }

    private static void AddBlock(
        DocComment result,
        string commandName,
        string description,
        CCompilationUnit? compilationUnit)
    {
        switch (commandName.ToLowerInvariant())
        {
            case "brief":
            case "short":
                AddSummary(result, description);
                break;

            case "return":
            case "returns":
                if (!string.IsNullOrWhiteSpace(description))
                {
                    result.Return ??= new CommentReturn();
                    result.Return.Description = Combine(result.Return.Description, description);
                }
                break;

            case "details":
            case "note":
            case "remark":
            case "remarks":
            case "warning":
            case "since":
                AddRemarks(result, description);
                break;

            case "see":
            case "sa":
                AddSeeAlso(result, description, compilationUnit);
                break;

            default:
                AddRemarks(result, description);
                break;
        }
    }

    private static void AddParagraph(DocComment result, string description)
    {
        AddSummary(result, description);
    }

    private static void AddSummary(DocComment result, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return;
        }

        result.Summary ??= new CommentSummery();
        result.Summary.Description = CombineParagraphs(result.Summary.Description, description);
    }

    private static void AddParameter(DocComment result, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
        {
            return;
        }

        var existing = result.Parameters.FirstOrDefault(parameter => parameter.Name == name);
        if (existing is not null)
        {
            existing.Description = Combine(existing.Description, description);
            return;
        }

        result.Parameters.Add(new CommentParameter
        {
            Name = name,
            Description = description,
        });
    }

    private static void AddRemarks(DocComment result, string description)
    {
        if (!string.IsNullOrWhiteSpace(description))
        {
            result.Remarks.Add(new CommentRemarks { Description = description });
        }
    }

    private static void AddSeeAlso(
        DocComment result,
        string reference,
        CCompilationUnit? compilationUnit)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            return;
        }

        var cItem = ResolveReference(compilationUnit, reference);
        LazyFormatString lazyReference = cItem is null
            ? reference
            : new LazyFormatString("{0}", (Func<string>)(() => GetCSFullName(cItem) ?? reference));

        result.SeeAlso.Add(new CommentSeeAlso { Reference = lazyReference });
    }

    private static BaseCAstItem? ResolveReference(CCompilationUnit? compilationUnit, string reference)
    {
        return compilationUnit?.GetFunctionByName(reference)
            ?? compilationUnit?.GetTypeByName(reference) as BaseCAstItem
            ?? compilationUnit?.GetConstantByName(reference) as BaseCAstItem
            ?? compilationUnit?.GetEnumFieldByName(reference) as BaseCAstItem;
    }

    private static string? GetCSFullName(BaseCAstItem cItem)
    {
        foreach (var derivative in cItem.Derivatives)
        {
            switch (derivative)
            {
                case CSMethod method when method.Parent is not null:
                    return method.GetFullName();
                case CSField field when field.Parent is not null:
                    return field.GetFullName();
                case BaseCSType type:
                    return type.GetFullName();
                case CSEnumField enumField when enumField.Parent is not null:
                    return enumField.GetFullName();
            }
        }

        return null;
    }

    private static string GetText(CBaseComment comment, IEnumerable<string>? arguments = null)
    {
        var builder = new StringBuilder();

        if (arguments is not null)
        {
            foreach (var argument in arguments)
            {
                AppendText(builder, argument);
            }
        }

        foreach (var child in comment.Children)
        {
            AppendNodeText(builder, child);
        }

        return Normalize(builder.ToString());
    }

    private static void AppendNodeText(StringBuilder builder, CBaseComment comment)
    {
        switch (comment)
        {
            case CTextComment text:
                AppendText(builder, text.Text);
                break;

            case CInlineCommandComment inline:
                foreach (var argument in inline.Arguments)
                {
                    AppendText(builder, argument);
                }
                break;

            case CVerbatimLineComment verbatimLine:
                AppendLine(builder, verbatimLine.Text);
                break;

            case CVerbatimBlockLineComment verbatimBlockLine:
                AppendLine(builder, verbatimBlockLine.Text);
                break;

            case CBlockCommandComment block:
                foreach (var argument in block.Arguments)
                {
                    AppendText(builder, argument);
                }
                break;
        }

        foreach (var child in comment.Children)
        {
            AppendNodeText(builder, child);
        }
    }

    private static void AppendText(StringBuilder builder, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        if (builder.Length > 0 && !char.IsWhiteSpace(builder[^1]) && !char.IsPunctuation(text[0]))
        {
            builder.Append(' ');
        }

        builder.Append(text.Trim());
    }

    private static void AppendLine(StringBuilder builder, string text)
    {
        if (builder.Length > 0)
        {
            builder.AppendLine();
        }

        builder.Append(text.Trim());
    }

    private static string Normalize(string text)
    {
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n')
            .Split('\n')
            .Select(line => string.Join(' ', line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)))
            .Where(line => line.Length > 0);

        return string.Join(Environment.NewLine, lines);
    }

    private static string Combine(string? current, string addition)
    {
        return string.IsNullOrWhiteSpace(current)
            ? addition
            : $"{current}{Environment.NewLine}{addition}";
    }

    private static string CombineParagraphs(string? current, string addition)
    {
        return string.IsNullOrWhiteSpace(current)
            ? addition
            : $"{current}{Environment.NewLine}{Environment.NewLine}{addition}";
    }
}
