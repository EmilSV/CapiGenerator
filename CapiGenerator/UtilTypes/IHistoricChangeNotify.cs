namespace CapiGenerator.UtilTypes;

public interface IHistoricChangeNotify<T> : IHistoricChangeNotify
{
    public new event Action<T>? OnChange;
}

public interface IHistoricChangeNotify
{
    public event Action? OnChange;
}