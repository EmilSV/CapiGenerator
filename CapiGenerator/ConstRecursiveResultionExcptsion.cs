
using CapiGenerator.Model;

namespace CapiGenerator;


public class ConstRecursiveResolutionException : Exception
{
    public Constant[] ConstChain { get; }
    public ConstRecursiveResolutionException(Constant[] constChain, string message) : base(message)
    {
        ConstChain = constChain;
    }
}