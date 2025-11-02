namespace ToksikApp.Contracts.GetAiSummary.Requests.Query;

public class GetAiEmotionsSummaryQueryRequestItem
{
    public string? Emotion { get; set; }

    public string? Reason { get; set; }

    public string? Solution { get; set; }

    public string? Intensity { get; set; }

    public string? Notes { get; set; }

    public string? Tags { get; set; }

    public string? CreatedAt { get; set; }

    public string? UpdatedAt { get; set; }
}