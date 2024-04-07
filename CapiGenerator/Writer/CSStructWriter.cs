using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;

public class CSStructWriter : BaseCSStructWriter
{
    public override async Task Write(CSStruct csStruct, CSWriteConfig writeConfig)
    {
        var structName = csStruct.Name;
        var structFields = csStruct.Fields;

        using var stream = new StreamWriter(Path.Combine(writeConfig.OutputDirectory, $"{structName}.cs"));

        foreach (var usingNamespace in writeConfig.Usings)
        {
            stream.WriteLine($"using {usingNamespace};");
        }

        await stream.FlushAsync();

        if (csStruct.Namespace is not null)
        {
            stream.WriteLine($"namespace {csStruct.Namespace};");
        }

        stream.WriteLine($"public struct {structName}");
        stream.WriteLine("{");

        await stream.FlushAsync();

        foreach (var structField in structFields)
        {
            WriteToStream(stream, structField);
            await stream.FlushAsync();
        }

        foreach (var structMethod in csStruct.Methods)
        {
            WriteToStream(stream, structMethod);
            await stream.FlushAsync();
        }

        stream.WriteLine("}");

    }
    private static void WriteToStream(StreamWriter writer, CSMethod method)
    {
        writer.Write(method.AccessModifier.Value switch
        {
            CSAccessModifier.Public => "public",
            CSAccessModifier.Private => "private",
            CSAccessModifier.Protected => "protected",
            CSAccessModifier.Internal => "internal",
            CSAccessModifier.ProtectedInternal => "protected internal",
            CSAccessModifier.PrivateProtected => "private protected",
            _ => throw new ArgumentOutOfRangeException("method.AccessModifier.Value")
        });

        if (method.IsStatic.Value)
        {
            writer.Write(" static");
        }

        if (method.IsExtern.Value)
        {
            writer.Write(" extern");
        }

        writer.Write(" ");
        writer.Write(method.ReturnType.Value!.ToString());
        writer.Write(" ");
        writer.Write(method.Name.Value);
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
        writer.WriteLine();
        if (method.Body.Value is not null)
        {
            writer.WriteLine("{");
            writer.Write(method.Body.Value);
            writer.WriteLine("}");
        }
        else
        {
            writer.WriteLine(";");
        }
        writer.WriteLine();
    }

    private static void WriteToStream(StreamWriter writer, CSField field)
    {
        writer.Write(field.AccessModifier.Value switch
        {
            CSAccessModifier.Public => "public",
            CSAccessModifier.Private => "private",
            CSAccessModifier.Protected => "protected",
            CSAccessModifier.Internal => "internal",
            CSAccessModifier.ProtectedInternal => "protected internal",
            CSAccessModifier.PrivateProtected => "private protected",
            _ => throw new ArgumentOutOfRangeException("field.AccessModifier.Value")
        });

        if (field.IsStatic.Value)
        {
            writer.Write(" static");
        }

        if (field.IsConst.Value)
        {
            writer.Write(" const");
        }

        if (field.IsReadOnly.Value)
        {
            writer.Write(" readonly");
        }

        writer.Write(" ");
        writer.Write(field.Type.Value!.ToString());

        writer.Write(" ");
        writer.Write(field.Name.Value);

        WriteToStreamGetterSetter(writer, field.GetterBody, field.SetterBody);

        if (field.DefaultValue.Value != CSDefaultValue.NullValue)
        {
            writer.Write(" = ");
            WriteToStream(writer, field.DefaultValue.Value);
        }

        writer.Write(";");
        writer.WriteLine();
    }

    private static void WriteToStream(StreamWriter writer, CSDefaultValue defaultValue)
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
            writer.Write($"\"{stringValue}\"");
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

    private static void WriteToStreamGetterSetter(
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
}