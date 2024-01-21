using CapiGenerator.CModel;
using CapiGenerator.CSharpEnrichingData;
using CapiGenerator.Enricher;
using CapiGenerator.Parser;

namespace CapiGenerator.CSharpEnricher
{
    public class StaticClassEnricher(
        string namespaceName,
        string staticClassName,
        Func<BaseCAstItem, bool> predicate) : BaseEnricher
    {
        private readonly StaticClassData data = new()
        {
            NamespaceName = namespaceName,
            StaticClassName = staticClassName
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