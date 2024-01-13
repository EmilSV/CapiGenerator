using CapiGenerator.Model;
using CapiGenerator.Parser;

namespace CapiGenerator.Type;


public class FunctionType(TypeInstance returnType, ReadOnlySpan<CParameter> parameters)
    : BaseCAstItem(compilationUnitId)
{
    public class Parameter
    {
        public readonly string Name = name;
        private TypeInstance _type = type;
        public TypeInstance GetParameterType() => _type;

        public void OnSecondPass(CCompilationUnit compilationUnit)
        {
            if (_type.IsCompletedType)
            {
                return;
            }

            var typeName = _type.TypeName;
            if (typeName != null)
            {
                var newType = compilationUnit.GetTypeByName(typeName);
                if (newType != null)
                {
                    _type = _type.NewWithType(newType);
                }
            }
        }
    }

    private readonly CParameter[] _parameters = parameters.ToArray();
    private TypeInstance _returnType = returnType;

    public string Name => name;
    public TypeInstance ReturnType => _returnType;
    public ReadOnlySpan<CParameter> Parameters => _parameters;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var parameter in Parameters)
        {
            parameter.OnSecondPass(compilationUnit);
        }

        if (!_returnType.IsCompletedType && _returnType.TypeName != null)
        {
            var returnTypeName = _returnType.TypeName;
            var newReturnType = compilationUnit.GetTypeByName(returnTypeName);
            if (newReturnType != null)
            {
                _returnType = _returnType.NewWithType(newReturnType);
            }
        }
    }
}