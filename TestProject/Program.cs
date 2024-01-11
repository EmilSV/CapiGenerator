
using CapiGenerator;
using CapiGenerator.Type;
using CppAst;


var numberTypes = PrimitiveType.GetAllTypes();

foreach (var item in numberTypes)
{
    Console.WriteLine($"{item.Name} {item.KindValue}");
}

return 0;
