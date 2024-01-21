using CapiGenerator.CModel;
using CapiGenerator.CSharpEnrichingData;
using CapiGenerator.Enricher;
using CapiGenerator.Parser;

namespace CapiGenerator.CSharpEnricher
{
    public class StructEnricher(
        string namespaceName,
        string structName,
        Func<BaseCAstItem, bool> predicate) : BaseEnricher
    {
        private readonly StructData data = new()
        {
            NamespaceName = namespaceName,
            StructName = structName
        };

        public override void Enrich(ReadOnlySpan<CCompilationUnit> compilationUnit)
        {
            foreach (var unit in compilationUnit)
            {
                foreach (var item in unit.GetStructEnumerable())
                {
                    if (predicate(item))
                    {
                        item.AddEnrichingData(data);
                    }
                }

                foreach (var item in unit.GetEnumEnumerable())
                {
                    if (predicate(item))
                    {
                        item.AddEnrichingData(data);
                    }
                }

                foreach (var item in unit.GetFunctionEnumerable())
                {
                    if (predicate(item))
                    {
                        item.AddEnrichingData(data);
                    }
                }

                foreach (var item in unit.GetTypedefEnumerable())
                {
                    if (predicate(item))
                    {
                        item.AddEnrichingData(data);
                    }
                }

                foreach (var item in unit.GetConstantEnumerable())
                {
                    if (predicate(item))
                    {
                        item.AddEnrichingData(data);
                    }
                }
            }
        }
    }
}