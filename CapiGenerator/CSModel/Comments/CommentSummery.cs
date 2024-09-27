namespace CapiGenerator.CSModel.Comments;

public sealed class CommentSummery
{
    public string? SummaryText { get; set; }
    public List<CommentParameter> Params { get; set; } = new();
    public string? ReturnsText { get; set; }

    public bool HasValue()
    {
        bool hasSummary = SummaryText != null && SummaryText.Length > 0;
        bool hasParams = Params.Count > 0;
        bool hasReturns = ReturnsText != null && ReturnsText.Length > 0;
        return hasSummary || hasParams || hasReturns;
   }
}