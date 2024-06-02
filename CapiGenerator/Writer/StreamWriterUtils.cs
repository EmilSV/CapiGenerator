using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;

public static class StreamWriterUtils
{
    public static void WriteToStream(StreamWriter writer, CSMethod method)
    {

        foreach (var attribute in method.Attributes)
        {
            WriteToStream(writer, attribute);
            writer.WriteLine();
            writer.Write("\t");
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

        writer.Write(" ");
        writer.Write(method.ReturnType!.ToString());
        writer.Write(" ");
        writer.Write(method.Name);
        writer.Write("(");

        for (var i = 0; i < method.Parameters.Count; i++)
        {
            var parameter = method.Parameters[i];
            writer.Write(parameter.Type.ToString());
            writer.Write(" ");
            writer.Write(parameter.Name);
            if (i < method.Parameters.Count - 1)
            {
                writer.Write(",");
            }
        }

        writer.Write(")");
        if (method.Body is not null)
        {
            writer.WriteLine();
            writer.WriteLine("{");
            writer.Write(method.Body);
            writer.WriteLine("}");
        }
        else
        {
            writer.Write(";");
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
}