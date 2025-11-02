namespace ToksikApp.Contracts.GetAiSummary.Responses.Query;

public class GetAiEmotionsSummaryQueryResponse
{
    public List<string>? Triggers { get; set; }

    public List<string>? Patterns { get; set; }

    public string? Advice { get; set; }
}