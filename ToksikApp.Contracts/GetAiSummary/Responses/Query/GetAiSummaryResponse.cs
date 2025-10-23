namespace ToksikApp.Contracts.GetAiSummary.Responses.Query;

public class GetAiSummaryResponse
{
    public string? Summary { get; set; }

    public GetAiSummaryResponse(string summary)
    {
        Summary = summary;
    }
}