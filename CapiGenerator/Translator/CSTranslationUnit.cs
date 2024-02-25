using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;

namespace CapiGenerator.Translator;

public sealed class CSTranslationUnit :
    IResolver<ICSType, ICType>,
    IResolver<CSField, CConstant>
{
    private class TranslatorOutputChannel(CSTranslationUnit translationUnit) : BaseTranslatorOutputChannel
    {
        private List<CSEnum> _enums = new();
        private List<CSStaticClass> _staticClasses = new();
        private List<CSStruct> _structs = new();

        public override void OnReceiveEnum(ReadOnlySpan<CSEnum> enums)
        {
            _enums.AddRange(enums);
            foreach (var item in enums)
            {
                translationUnit._enumByName.Add(item.Name, item);
            }
        }

        public override void OnReceiveStaticClass(ReadOnlySpan<CSStaticClass> staticClasses)
        {
            _staticClasses.AddRange(_staticClasses);
            foreach (var staticClass in _staticClasses)
            {
                translationUnit._staticClassesByName.Add(staticClass.Name, staticClass);
                foreach (var field in staticClass.Fields)
                {
                    var cAst = field.EnrichingDataStore.Get<CSTranslationFromCAstData>();
                    if (cAst?.AstItem is not CConstant cConstant)
                    {
                        continue;
                    }

                    translationUnit._felidByCConst.Add(cConstant, field);
                }
            }
        }

        public override void OnReceiveStruct(ReadOnlySpan<CSStruct> structs)
        {
            throw new NotImplementedException();
        }

    }

    private Dictionary<string, CSStaticClass> _staticClassesByName = new();
    private Dictionary<string, CSStruct> _structByName = new();
    private Dictionary<string, CSEnum> _enumByName = new();

    private Dictionary<CConstant, CSField> _felidByCConst = new();
    private Dictionary<ICType, ICSType> _csTypeByCType = new();


    ICSType? IResolver<ICSType, ICType>.Resolve(ICType key)
    {
        if (_csTypeByCType.TryGetValue(key, out var val))
        {
            return val;
        }

        return null;
    }

    CSField? IResolver<CSField, CConstant>.Resolve(CConstant key)
    {
        if (_felidByCConst.TryGetValue(key, out var val))
        {
            return val;
        }

        return null;
    }


}