using CapiGenerator.CSModel;

namespace CapiGenerator.XmlComments;

public interface IXmlCommentsTypeProvider
{
    IEnumerable<CSStaticClass> GetCSStaticClassesEnumerable();
    IEnumerable<CSEnum> GetCSEnumsEnumerable();
    IEnumerable<CSStruct> GetCSStructsEnumerable();
}
