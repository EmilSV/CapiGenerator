namespace CapiGenerator.UtilTypes;

public record struct InstanceId
{
    private static uint _nextId = 0;
    private static uint GetNextId()
    {
        return Interlocked.Increment(ref _nextId);
    }

    private readonly uint _id;
    public InstanceId()
    {
        _id = GetNextId();
    }

    public bool isValid => _id != 0;

    public static implicit operator uint(InstanceId id) => id._id;
}