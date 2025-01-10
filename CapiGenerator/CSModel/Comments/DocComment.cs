namespace CapiGenerator.CSModel.Comments;

public class DocComment
{
    public CommentSummery? Summary = null;
    public CommentValue? Value = null;
    public List<CommentParameter> Parameters { get; init; } = new();
    public List<CommentRemarks> Remarks { get; init; } = new();
    public CommentReturn? Return = null;

    public bool HasValue()
    {
        bool hasSummary = Summary?.HasValue() == true;
        bool hasValue = Value?.HasValue() == true;
        bool hasParams = Parameters.Count > 0;
        bool hasRemarks = Remarks.Count > 0;
        bool hasReturn = Return?.HasValue() == true;
        return hasSummary || hasValue || hasParams || hasRemarks || hasReturn;
    }
}