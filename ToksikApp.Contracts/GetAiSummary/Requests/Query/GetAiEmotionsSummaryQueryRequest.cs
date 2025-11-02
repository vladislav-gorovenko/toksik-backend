namespace ToksikApp.Contracts.GetAiSummary.Requests.Query;

public class GetAiEmotionsSummaryQueryRequest
{
    public List<GetAiEmotionsSummaryQueryRequestItem>? Records { get; set; }

    public string? Language { get; set; } = "English";
}