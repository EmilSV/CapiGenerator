using System.Text;
using CapiGenerator.CSModel;
using static CapiGenerator.Writer.StreamWriterUtils;

namespace CapiGenerator.Writer;

internal static class CSTypeDeclarationWriter
{
    public static Task WriteTypeDeclaration(StreamWriter stream, BaseCSType csType, int indentLevel = 0)
    {
        return csType switch
        {
            CSStruct csStruct => WriteStructDeclaration(stream, csStruct, indentLevel),
            CSEnum csEnum => WriteEnumDeclaration(stream, csEnum, indentLevel),
            CSStaticClass csStaticClass => WriteStaticClassDeclaration(stream, csStaticClass, indentLevel),
            _ => throw new NotSupportedException($"Unsupported C# type declaration: {csType.GetType().FullName}")
        };
    }

    public static async Task WriteStructDeclaration(StreamWriter stream, CSStruct csStruct, int indentLevel = 0)
    {
        await WriteCommentsAndAttributes(stream, csStruct, indentLevel);

        WriteIndent(stream, indentLevel);
        stream.Write(ToAccessModifierString(csStruct.AccessModifier));
        stream.Write(' ');

        if (csStruct.IsUnsafe)
        {
            stream.Write("unsafe ");
        }

        if (csStruct.IsPartial)
        {
            stream.Write("partial ");
        }

        if (csStruct.IsReadOnly)
        {
            stream.Write("readonly ");
        }

        stream.Write("struct ");
        stream.Write(csStruct.Name);

        if (csStruct.Interfaces.Count > 0)
        {
            stream.Write(" : ");
            bool first = true;
            foreach (var @interface in csStruct.Interfaces)
            {
                if (!first)
                {
                    stream.Write(", ");
                }

                stream.Write(@interface.ToString());
                first = false;
            }
        }

        stream.WriteLine();
        WriteIndent(stream, indentLevel);
        stream.WriteLine("{");

        foreach (var structField in csStruct.Fields)
        {
            await WriteIndented(stream, indentLevel + 1, writer => WriteToStream(writer, structField.Comments));
            await WriteIndented(stream, indentLevel + 1, writer =>
            {
                WriteToStream(writer, structField);
                return Task.CompletedTask;
            });
        }

        foreach (var constructor in csStruct.Constructors)
        {
            await WriteIndented(stream, indentLevel + 1, writer => WriteToStream(writer, constructor));
        }

        foreach (var structMethod in csStruct.Methods)
        {
            await WriteIndented(stream, indentLevel + 1, writer => WriteToStream(writer, structMethod.Comments));
            await WriteIndented(stream, indentLevel + 1, writer => WriteToStream(writer, structMethod));
        }

        foreach (var nestedType in csStruct.NestedTypes)
        {
            stream.WriteLine();
            await WriteTypeDeclaration(stream, nestedType, indentLevel + 1);
        }

        WriteIndent(stream, indentLevel);
        stream.WriteLine("}");
        await stream.FlushAsync();
    }

    public static async Task WriteEnumDeclaration(StreamWriter stream, CSEnum csEnum, int indentLevel = 0)
    {
        await WriteCommentsAndAttributes(stream, csEnum, indentLevel);

        WriteIndent(stream, indentLevel);
        stream.Write(ToAccessModifierString(csEnum.AccessModifier));
        stream.Write(" enum ");
        stream.Write(csEnum.Name);

        if (csEnum.Type.KindValue != CSPrimitiveType.Kind.Int)
        {
            stream.Write(" : ");
            stream.Write(csEnum.Type.Name);
        }

        stream.WriteLine();
        WriteIndent(stream, indentLevel);
        stream.WriteLine("{");

        foreach (var enumValue in csEnum.Values)
        {
            await WriteIndented(stream, indentLevel + 1, writer => WriteToStream(writer, enumValue.Comments));
            foreach (var attribute in enumValue.Attributes)
            {
                WriteIndent(stream, indentLevel + 1);
                WriteToStream(stream, attribute);
                stream.WriteLine();
            }

            WriteIndent(stream, indentLevel + 1);
            stream.WriteLine($"{enumValue.Name} = {enumValue.Expression},");
        }

        WriteIndent(stream, indentLevel);
        stream.WriteLine("}");
        await stream.FlushAsync();
    }

    public static async Task WriteStaticClassDeclaration(StreamWriter stream, CSStaticClass csStaticClass, int indentLevel = 0)
    {
        await WriteCommentsAndAttributes(stream, csStaticClass, indentLevel);

        WriteIndent(stream, indentLevel);
        stream.Write(ToAccessModifierString(csStaticClass.AccessModifier));
        stream.Write(' ');

        if (csStaticClass.IsUnsafe)
        {
            stream.Write("unsafe ");
        }

        stream.Write("static");
        if (csStaticClass.IsPartial)
        {
            stream.Write(" partial");
        }

        stream.Write(" class ");
        stream.WriteLine(csStaticClass.Name);
        WriteIndent(stream, indentLevel);
        stream.WriteLine("{");

        foreach (var classField in csStaticClass.Fields)
        {
            await WriteIndented(stream, indentLevel + 1, writer => WriteToStream(writer, classField.Comments));
            await WriteIndented(stream, indentLevel + 1, writer =>
            {
                WriteToStream(writer, classField);
                return Task.CompletedTask;
            });
        }

        if (csStaticClass.Fields.Count > 0 && csStaticClass.Methods.Count > 0)
        {
            stream.WriteLine();
        }

        foreach (var classMethod in csStaticClass.Methods)
        {
            await WriteIndented(stream, indentLevel + 1, writer => WriteToStream(writer, classMethod.Comments));
            await WriteIndented(stream, indentLevel + 1, writer => WriteToStream(writer, classMethod));
        }

        WriteIndent(stream, indentLevel);
        stream.WriteLine("}");
        await stream.FlushAsync();
    }

    private static async Task WriteCommentsAndAttributes(StreamWriter stream, BaseCSType type, int indentLevel)
    {
        await WriteIndented(stream, indentLevel, writer => WriteToStream(writer, type.Comments));

        foreach (var attribute in type.Attributes)
        {
            WriteIndent(stream, indentLevel);
            WriteToStream(stream, attribute);
            stream.WriteLine();
        }
    }

    private static async Task WriteIndented(StreamWriter stream, int indentLevel, Func<StreamWriter, Task> writeAction)
    {
        using var memoryStream = new MemoryStream();
        await using (var tempWriter = new StreamWriter(memoryStream, Encoding.UTF8, 1024, leaveOpen: true))
        {
            await writeAction(tempWriter);
            await tempWriter.FlushAsync();
        }

        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream, Encoding.UTF8);
        while (await reader.ReadLineAsync() is { } line)
        {
            if (line.Length > 0)
            {
                WriteIndent(stream, indentLevel);
            }

            stream.WriteLine(line);
        }
    }

    private static void WriteIndent(StreamWriter stream, int indentLevel)
    {
        for (int i = 0; i < indentLevel; i++)
        {
            stream.Write('\t');
        }
    }

    private static string ToAccessModifierString(CSAccessModifier accessModifier)
    {
        return accessModifier switch
        {
            CSAccessModifier.Public => "public",
            CSAccessModifier.Private => "private",
            CSAccessModifier.Protected => "protected",
            CSAccessModifier.Internal => "internal",
            CSAccessModifier.ProtectedInternal => "protected internal",
            CSAccessModifier.PrivateProtected => "private protected",
            _ => throw new ArgumentOutOfRangeException(nameof(accessModifier), accessModifier, null)
        };
    }
}
