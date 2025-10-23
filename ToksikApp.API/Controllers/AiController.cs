using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ToksikApp.Contracts.GetAiSummary.Responses.Query;

namespace ToksikApp.Controllers;

[ApiController]
[Route("ai")]
public class AiController : ControllerBase
{
    public HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AiController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet("generate-emotions-summary")]
    public async Task<GetAiSummaryResponse> GetAiSummary()
    {
        return new GetAiSummaryResponse("Hello world");
    }
}