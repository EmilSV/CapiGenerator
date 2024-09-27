using System.Reflection.Metadata;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.Writer;

public static class StreamWriterUtils
{
    public static async Task WriteToStream(StreamWriter writer, CSMethod method)
    {
        foreach (var attribute in method.Attributes)
        {
            WriteToStream(writer, attribute);
            writer.WriteLine();
            writer.Write('\t');
        }

        writer.Write(method.AccessModifier switch
        {
            CSAccessModifier.Public => "public",
            CSAccessModifier.Private => "private",
            CSAccessModifier.Protected => "protected",
            CSAccessModifier.Internal => "internal",
            CSAccessModifier.ProtectedInternal => "protected internal",
            CSAccessModifier.PrivateProtected => "private protected",
            _ => throw new ArgumentOutOfRangeException("method.AccessModifier.Value")
        });

        if (method.IsStatic)
        {
            writer.Write(" static");
        }

        if (method.IsExtern)
        {
            writer.Write(" extern");
        }

        if (method.IsOverride)
        {
            writer.Write(" override");
        }

        writer.Write(' ');

        if (method.OperatorModifier is CSMethodOperatorModifier.None or CSMethodOperatorModifier.Operator)
        {
            writer.Write(method.ReturnType!.ToString());
            writer.Write(' ');
        }

        if (method.OperatorModifier != CSMethodOperatorModifier.None)
        {
            writer.Write(method.OperatorModifier switch
            {
                CSMethodOperatorModifier.Explicit => "explicit operator ",
                CSMethodOperatorModifier.Implicit => "implicit operator ",
                CSMethodOperatorModifier.Operator => "operator ",
                _ => throw new NotImplementedException(),
            });
        }

        if (method.OperatorModifier is CSMethodOperatorModifier.Explicit or CSMethodOperatorModifier.Implicit)
        {
            writer.Write(method.ReturnType!.ToString());
            writer.Write(' ');
        }

        if (method.OperatorModifier is CSMethodOperatorModifier.None or CSMethodOperatorModifier.Operator)
        {
            writer.Write(method.Name);
        }

        await WriteParameters(writer, method.Parameters);
        if (method.Body is LazyFormatString body)
        {
            await WriteBody(writer, body);
        }
        else
        {
            writer.Write(';');
        }
        writer.WriteLine();
    }

    public static void WriteToStream(StreamWriter writer, CSField field)
    {
        writer.Write(field.AccessModifier switch
        {
            CSAccessModifier.Public => "public",
            CSAccessModifier.Private => "private",
            CSAccessModifier.Protected => "protected",
            CSAccessModifier.Internal => "internal",
            CSAccessModifier.ProtectedInternal => "protected internal",
            CSAccessModifier.PrivateProtected => "private protected",
            _ => throw new ArgumentOutOfRangeException("field.AccessModifier.Value")
        });

        if (field.IsStatic)
        {
            writer.Write(" static");
        }

        if (field.IsConst)
        {
            writer.Write(" const");
        }

        if (field.IsReadOnly)
        {
            writer.Write(" readonly");
        }

        if (field.IsRequired)
        {
            writer.Write(" required");
        }

        writer.Write(" ");
        writer.Write(field.Type.ToString());

        writer.Write(" ");
        writer.Write(field.Name);

        bool hasGetterOrSetter = HasGetterOrSetter(field);

        if (hasGetterOrSetter)
        {
            WriteToStreamGetterSetter(writer, field.GetterBody, field.SetterBody);
        }

        if (field.DefaultValue != CSDefaultValue.NullValue)
        {
            WriteToStream(writer, field.DefaultValue);
        }

        if (!hasGetterOrSetter)
        {
            writer.Write(";");
        }
        writer.WriteLine();
    }

    public static void WriteToStream(StreamWriter writer, CSDefaultValue defaultValue)
    {
        if (defaultValue == CSDefaultValue.NullValue)
        {
            return;
        }

        writer.Write(" = ");
        if (defaultValue.TryGetDouble(out var doubleValue))
        {
            writer.Write(doubleValue);
        }
        else if (defaultValue.TryGetUlong(out var unsignedIntValue))
        {
            writer.Write(unsignedIntValue);
        }
        else if (defaultValue.TryGetLong(out var signedIntValue))
        {
            writer.Write(signedIntValue);
        }
        else if (defaultValue.TryGetBool(out var boolValue))
        {
            writer.Write(boolValue ? "true" : "false");
        }
        else if (defaultValue.TryGetString(out var stringValue))
        {
            writer.Write($"{stringValue}");
        }
        else if (defaultValue.TryGetCSConstantExpression(out var csConstantExpression))
        {
            writer.Write(csConstantExpression.ToString());
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public static async Task WriteToStream(StreamWriter writer, CSConstructor constructor)
    {
        writer.WriteLine();

        writer.Write(constructor.AccessModifier switch
        {
            CSAccessModifier.Public => "public",
            CSAccessModifier.Private => "private",
            CSAccessModifier.Protected => "protected",
            CSAccessModifier.Internal => "internal",
            CSAccessModifier.ProtectedInternal => "protected internal",
            CSAccessModifier.PrivateProtected => "private protected",
            _ => throw new ArgumentOutOfRangeException("constructor.AccessModifier.Value")
        });

        writer.Write(" ");

        if (constructor.ParentType is null)
        {
            throw new InvalidOperationException("Parent type is not set");
        }

        writer.Write(constructor.ParentType.Name);
        await WriteParameters(writer, constructor.Parameters);
        await WriteBody(writer, constructor.Body ?? "");
        writer.WriteLine();
    }

    public static void WriteToStreamGetterSetter(
        StreamWriter writer, CSPropertyBody? getterBody, CSPropertyBody? setterBody)
    {
        if (getterBody is null && setterBody is null)
        {
            return;
        }

        writer.Write("{");
        if (getterBody is not null)
        {
            var code = getterBody.Code;
            if (code is null)
            {
                writer.Write("get;");
            }
            else
            {
                writer.WriteLine();
                writer.Write("get");
                writer.Write(code);
                writer.WriteLine();
            }
        }

        if (setterBody is not null)
        {
            var code = setterBody.Code;
            if (code is null)
            {
                writer.Write("set;");
            }
            else
            {
                writer.WriteLine();
                writer.Write("set");
                writer.Write(code);
                writer.WriteLine();
            }
        }
        writer.Write("}");
    }

    public static void WriteToStream(StreamWriter writer, BaseCSAttribute attribute)
    {
        writer.Write("[");
        writer.Write(attribute.GetFullAttributeName());

        if (attribute.CtorArgs.Length == 0 && attribute.Parameters.Count == 0)
        {
            writer.Write("]");
            return;
        }

        writer.Write("(");

        for (int i = 0; i < attribute.CtorArgs.Length; i++)
        {
            var argument = attribute.CtorArgs[i];
            writer.Write(argument);
            if (i < attribute.CtorArgs.Length - 1 || attribute.Parameters.Count > 0)
            {
                writer.Write(", ");
            }
        }

        using (var iter = attribute.Parameters.GetEnumerator())
        {
            if (iter.MoveNext())
            {
                var current = iter.Current;
                while (iter.MoveNext())
                {
                    writer.Write($"{current.Key} = {current.Value}");
                    writer.Write(", ");
                    current = iter.Current;
                }
                writer.Write($"{current.Key} = {current.Value}");
            }
        }
        writer.Write(")");

        writer.Write("]");
    }

    public static bool HasGetterOrSetter(CSField field)
    {
        return field.GetterBody is not null || field.SetterBody is not null;
    }

    public static async Task WriteParameters(StreamWriter writer, IEnumerable<CSParameter> parameters)
    {
        writer.Write("(");
        bool first = true;
        foreach (var parameter in parameters)
        {
            if (!first)
            {
                writer.Write(", ");
            }
            first = false;
            writer.Write(parameter.Type.ToString());
            writer.Write(' ');
            writer.Write(parameter.Name);
            WriteToStream(writer, parameter.DefaultValue);
        }
        writer.Write(")");

        await writer.FlushAsync();
    }

    public static async Task WriteBody(StreamWriter writer, LazyFormatString body)
    {
        string bodyString = body.ToString();

        if (bodyString.TrimStart().StartsWith("=>"))
        {
            writer.Write(' ');
            writer.Write(body.ToString().TrimStart());
        }
        else
        {
            writer.WriteLine();
            writer.WriteLine('{');
            writer.Write(body.ToString());
            writer.WriteLine('}');
        }

        await writer.FlushAsync();
    }

    public static async Task WriteToStream(StreamWriter writer, CommentSummery? comment)
    {
        if (comment == null || !comment.HasValue())
        {
            return;
        }

        writer.WriteLine("/// <summary>");

        if (comment.SummaryText != null && comment.SummaryText.Length != 0)
        {
            foreach (var commentLine in comment.SummaryText.Split('\n'))
            {
                writer.WriteLine($"/// {commentLine}");
            }
        }

        await writer.FlushAsync();

        writer.WriteLine("/// </summary>");

        foreach (var param in comment.Params)
        {
            writer.WriteLine($"/// <param name=\"{param.Name}\">{param.Description}</param>");
        }

        if (comment.ReturnsText != null && comment.ReturnsText.Length != 0)
        {
            writer.WriteLine($"/// <returns>{comment.ReturnsText}</returns>");
        }
        
        await writer.FlushAsync();
    }
}