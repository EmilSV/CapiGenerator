namespace CapiGenerator;

public sealed class ResoleRef<TOutput, TKey>(TKey key)
{
    public TKey GetResolveKey() => key;
    
}