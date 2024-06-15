using System.Runtime.CompilerServices;

namespace CapiGenerator.UtilTypes;

public readonly struct LazyFormatString(string format, params object[] args)
{
    public readonly string Format = format;
    private readonly object[] _args = args;

    public override string ToString()
    {
        if (_args.Length == 0)
        {
            return Format;
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

        return string.Format(Format, args);
    }

    public static implicit operator LazyFormatString(string format) => new(format);
}