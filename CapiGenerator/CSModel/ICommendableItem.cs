using CapiGenerator.CSModel.Comments;

namespace CapiGenerator.CSModel;

public interface ICommendableItem
{
    public DocComment? Comments { get; set; }
}