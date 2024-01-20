using CapiGenerator.CSharpEnrichingData;
using CapiGenerator.Enricher;
using CapiGenerator.Parser;

namespace CapiGenerator.CSharpEnricher
{
    public class NamespaceEnricher(string namespaceName) : BaseEnricher
    {
        public override void Enrich(ReadOnlySpan<CCompilationUnit> compilationUnit)
        {
            foreach (var unit in compilationUnit)
            {
                foreach (var item in unit.GetStructEnumerable())
                {
                    item.AddEnrichingData(new NamespaceData
                    {
                        NamespaceName = namespaceName
                    });
                }

                foreach (var item in unit.GetEnumEnumerable())
                {
                    item.AddEnrichingData(new NamespaceData
                    {
                        NamespaceName = namespaceName
                    });
                }

                foreach (var item in unit.GetFunctionEnumerable())
                {
                    item.AddEnrichingData(new NamespaceData
                    {
                        NamespaceName = namespaceName
                    });
                }

                foreach (var item in unit.GetTypedefEnumerable())
                {
                    item.AddEnrichingData(new NamespaceData
                    {
                        NamespaceName = namespaceName
                    });
                }

                foreach (var item in unit.GetConstantEnumerable())
                {
                    item.AddEnrichingData(new NamespaceData
                    {
                        NamespaceName = namespaceName
                    });
                }
            }
        }
    }
}