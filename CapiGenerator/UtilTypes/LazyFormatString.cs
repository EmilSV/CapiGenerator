using System.Runtime.CompilerServices;

namespace CapiGenerator.UtilTypes;

public readonly record struct LazyFormatString
{
    public LazyFormatString(string format, params object[] args)
    {
        _format = format;
        _args = args;
    }

    private readonly string _format;
    private readonly object[] _args;

    public override string ToString()
    {
        if (_args.Length == 0)
        {
            return _format;
        }

        var args = new object[_args.Length];
        for (int i = 0; i < _args.Length; i++)
        {
            args[i] = _args[i] switch
            {
                Func<string> func => func(),
                _ => _args[i]
            };
        }

        return string.Format(_format, args);
    }

    public static implicit operator LazyFormatString(string format) => new(format);
}