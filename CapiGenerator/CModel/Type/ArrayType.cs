using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator.CModel.Type;

public class ArrayType(ArrayType.Size size) : CTypeModifier
{
    public readonly record struct Size
    {
        private readonly BaseCConstant? constant;
        private readonly uint? value;

        public Size(BaseCConstant constant)
        {
            this.constant = constant;
            value = null;
        }

        public Size(uint value)
        {
            this.value = value;
            constant = null;
        }

        public bool IsConstant => constant != null;
        public bool IsValue => value != null;

        public bool TryAsConstant([MaybeNullWhen(false)] out BaseCConstant constant)
        {
            if (IsConstant)
            {
                constant = this.constant!;
                return true;
            }

            constant = default;
            return false;
        }

        public bool TryAsValue(out uint value)
        {
            if (IsValue)
            {
                value = this.value!.Value;
                return true;
            }

            value = default;
            return false;
        }

        public override string ToString()
        {
            if (IsConstant)
            {
                return constant?.ToString() ?? "null";
            }

            return value?.ToString() ?? "null";
        }

        public static implicit operator Size(BaseCConstant constant) => new(constant);
        public static implicit operator Size(uint value) => new(value);
        public static implicit operator Size(int value) => new((uint)value);
    }

    private string? _typeName;
    public Size SizeValue => size;

    public override string GetTypeString()
    {
        if (_typeName == null)
        {
            string sizeString = size switch
            {
                Size size when size.TryAsConstant(out var constant) => constant.Name,
                Size size when size.TryAsValue(out var value) => value.ToString(),
                _ => throw new InvalidOperationException()
            };

            _typeName = $"[{sizeString}]";
        }

        return _typeName;
    }
}