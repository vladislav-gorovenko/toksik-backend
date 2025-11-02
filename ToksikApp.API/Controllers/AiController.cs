using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using ToksikApp.Contracts.GetAiSummary.Requests.Query;
using ToksikApp.Contracts.GetAiSummary.Responses.Query;

namespace ToksikApp.Controllers;

[ApiController]
[Route("ai")]
public class AiController : ControllerBase
{
    private readonly ChatClient _chatClient;

    public AiController(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        _chatClient = new ChatClient("gpt-4o-mini", apiKey);
    }

    [HttpPost("analyze-emotions")]
    public async Task<ActionResult<GetAiEmotionsSummaryQueryResponse>> AnalyzeEmotions(
        [FromBody] GetAiEmotionsSummaryQueryRequest request)
    {
        if (request.Records == null || request.Records.Count == 0)
            return BadRequest("No records provided.");

        var userText = string.Join("\n", request.Records.Select(r =>
            string.Join("; ", new[]
            {
                r.Emotion is { Length: > 0 } ? $"Emotion: {r.Emotion}" : null,
                r.Reason is { Length: > 0 } ? $"Reason: {r.Reason}" : null,
                r.Solution is { Length: > 0 } ? $"Solution: {r.Solution}" : null,
                r.Intensity is { Length: > 0 } ? $"Intensity: {r.Intensity}" : null,
                r.Notes is { Length: > 0 } ? $"Notes: {r.Notes}" : null,
                r.Tags is { Length: > 0 } ? $"Tags: {r.Tags}" : null
            }.Where(x => x != null)).Trim()));

        var messages = new ChatMessage[]
        {
            new SystemChatMessage(
                "You are an emotion analysis expert. Analyze the emotional content of the text and provide a summary of the emotions detected."),
            new UserChatMessage(userText)
        };

        var schemaJson = """
                         {
                           "type": "object",
                           "properties": {
                             "triggers": {
                               "type": "array",
                               "items": { "type": "string" },
                               "maxItems": 10
                             },
                             "patterns": {
                               "type": "array",
                               "items": { "type": "string" },
                               "maxItems": 10
                             },
                             "advice": {
                               "type": "string"
                             }
                           },
                           "required": ["triggers", "patterns", "advice"],
                           "additionalProperties": false
                         }
                         """;

        var responseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
            jsonSchemaFormatName: "AiSummarySchema",
            jsonSchema: BinaryData.FromString(schemaJson),
            jsonSchemaIsStrict: true
        );

        var completion = await _chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
        {
            ResponseFormat = responseFormat
        });

        var json = completion.Value.Content[0].Text;
        var result = JsonSerializer.Deserialize<GetAiEmotionsSummaryQueryResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result == null)
            return StatusCode(500, "Failed to parse AI response.");

        return Ok(result);
    }
}