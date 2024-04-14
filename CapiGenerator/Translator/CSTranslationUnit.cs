using System.Runtime.InteropServices;
using CapiGenerator.CModel;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.Translator;

public sealed class CSTranslationUnit :
    IResolver<ICSType, ICType>,
    IResolver<ICSFieldLike, ICConstAssignable>
{
    private class TranslatorOutputChannel(CSTranslationUnit translationUnit) : BaseTranslatorOutputChannel
    {
        private readonly List<CSEnum> _enums = [];
        private readonly List<CSStaticClass> _staticClasses = [];
        private readonly List<CSStruct> _structs = [];
        private bool _isClosed;

        public override bool IsClosed => _isClosed;

        public override void OnReceiveEnum(ReadOnlySpan<CSEnum> enums)
        {
            if (_isClosed)
            {
                throw new InvalidOperationException("Channel is closed");
            }

            _enums.AddRange(enums);
            foreach (var item in enums)
            {
                translationUnit._enumByName.Add(item.FullName, item);
                var cAstType = item.EnrichingDataStore.Get<CSTranslationFromCAstData>()?.AstItem;
                if (cAstType is ICType cType)
                {
                    translationUnit._csTypeByCType.Add(cType, item);
                }
            }
        }

        public override void OnReceiveStaticClass(ReadOnlySpan<CSStaticClass> staticClasses)
        {
            if (_isClosed)
            {
                throw new InvalidOperationException("Channel is closed");
            }

            _staticClasses.AddRange(_staticClasses);
            foreach (var staticClass in _staticClasses)
            {
                translationUnit._staticClassesByName.Add(staticClass.FullName, staticClass);
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
            if (_isClosed)
            {
                throw new InvalidOperationException("Channel is closed");
            }

            _structs.AddRange(structs);
            foreach (var item in structs)
            {
                translationUnit._structByName.Add(item.FullName, item);
                var cAstType = item.EnrichingDataStore.Get<CSTranslationFromCAstData>()?.AstItem;
                if (cAstType is ICType cType)
                {
                    translationUnit._csTypeByCType.Add(cType, item);
                }

                foreach (var field in item.Fields)
                {
                    var cAst = field.EnrichingDataStore.Get<CSTranslationFromCAstData>()?.AstItem;
                    if (cAst is not CConstant cConstant)
                    {
                        continue;
                    }

                    translationUnit._felidByCConst.Add(cConstant, field);
                }
            }
        }

        public void Close()
        {
            _isClosed = true;
        }

        public TranslatorInputChannel CreateInputChannel()
        {
            if (!_isClosed)
            {
                throw new InvalidOperationException("Channel is not closed");
            }

            return new TranslatorInputChannel(_enums, _staticClasses, _structs);
        }
    }

    private class TranslatorInputChannel(
        List<CSEnum> enums,
        List<CSStaticClass> staticClasses,
        List<CSStruct> structs
    ) : BaseTranslatorInputChannel
    {

        public override ReadOnlySpan<CSEnum> GetEnums()
        {
            return CollectionsMarshal.AsSpan(enums);
        }

        public override ReadOnlySpan<CSStaticClass> GetStaticClasses()
        {
            return CollectionsMarshal.AsSpan(staticClasses);
        }

        public override ReadOnlySpan<CSStruct> GetStructs()
        {
            return CollectionsMarshal.AsSpan(structs);
        }
    }

    private readonly Dictionary<string, CSStaticClass> _staticClassesByName = [];
    private readonly Dictionary<string, CSStruct> _structByName = [];
    private readonly Dictionary<string, CSEnum> _enumByName = [];

    private readonly Dictionary<ICConstAssignable, ICSFieldLike> _felidByCConst = [];
    private readonly Dictionary<ICType, ICSType> _csTypeByCType = [];

    private readonly Dictionary<string, ICSType> _csTypeByCTypeName = [];
    private readonly List<BaseTranslator> _translators = [];

    public CSTranslationUnit()
    {
        BuildInTranslation.AddTranslation(this);
    }


    ICSType? IResolver<ICSType, ICType>.Resolve(ICType key)
    {
        if (_csTypeByCType.TryGetValue(key, out var val))
        {
            return val;
        }

        return null;
    }
    ICSFieldLike? IResolver<ICSFieldLike, ICConstAssignable>.Resolve(ICConstAssignable key)
    {
        if (_felidByCConst.TryGetValue(key, out var val))
        {
            return val;
        }

        return null;
    }

    public CSTranslationUnit AddPredefinedTranslation(ICType cType, ICSType csType)
    {
        _csTypeByCType.Add(cType, csType);
        return this;
    }

    public CSTranslationUnit AddPredefinedTranslation(string cTypeName, ICSType csType)
    {
        _csTypeByCTypeName.Add(cTypeName, csType);
        return this;
    }

    public CSTranslationUnit BanType(ICType cType)
    {
        _csTypeByCType.Add(cType, CSBannedType.Instance);
        return this;
    }

    public bool TryOverrideBannedType(ICType cType, ICSType csType)
    {
        if (_csTypeByCType.TryGetValue(cType, out var bannedType) && bannedType == CSBannedType.Instance)
        {
            _csTypeByCType[cType] = csType;
            return true;
        }

        return false;
    }

    public bool IsTypeBanned(ICType cType) =>
        _csTypeByCType.TryGetValue(cType, out var csType) && csType == CSBannedType.Instance;


    public bool IsTypeTranslated(ICType cType) =>
        _csTypeByCType.ContainsKey(cType);


    public CSTranslationUnit AddTranslator(BaseTranslator translator)
    {
        _translators.Add(translator);
        return this;
    }

    public CSTranslationUnit AddTranslator(ReadOnlySpan<BaseTranslator> translator)
    {
        _translators.AddRange(translator);
        return this;
    }


    public void Translate(ReadOnlySpan<CCompilationUnit> compilationUnits)
    {
        List<(TranslatorOutputChannel, BaseTranslator)> channels = [];

        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var structItem in compilationUnit.GetEnumEnumerable())
            {
                if (_csTypeByCTypeName.Remove(structItem.Name, out var csType))
                {
                    _csTypeByCType.Add(structItem, csType);
                    continue;
                }
            }

            foreach (var enumItem in compilationUnit.GetEnumEnumerable())
            {
                if (_csTypeByCTypeName.Remove(enumItem.Name, out var csType))
                {
                    _csTypeByCType.Add(enumItem, csType);
                    continue;
                }
            }
        }

        _csTypeByCTypeName.Clear();

        foreach (var translator in _translators)
        {
            var outputChannel = new TranslatorOutputChannel(this);
            channels.Add((outputChannel, translator));
            translator.FirstPass(this, compilationUnits, outputChannel);
        }

        foreach (var (channel, translator) in channels)
        {
            channel.Close();
            translator.SecondPass(this, channel.CreateInputChannel());
        }
    }

    public IEnumerable<CSStaticClass> GetCSStaticClassesEnumerable() => _staticClassesByName.Values;
    public IEnumerable<CSStruct> GetCSStructEnumerable() => _structByName.Values;
    public IEnumerable<CSEnum> GetCSEnumEnumerable() => _enumByName.Values;
}