namespace CapiGenerator;


public class RecursiveResolutionException : InvalidOperationException
{
    public RecursiveResolutionException(string message) : base(message)
    {
    }
}