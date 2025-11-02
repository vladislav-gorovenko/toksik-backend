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
        _chatClient = new ChatClient("gpt-4o", apiKey);
    }

    [HttpPost("emotions-summary")]
    public async Task<ActionResult<GetAiEmotionsSummaryQueryResponse>> AnalyzeEmotions(
        [FromBody] GetAiEmotionsSummaryQueryRequest request)
    {
        if (request.Records == null || request.Records.Count == 0)
            return BadRequest("No records provided.");

        const int maxRecordLength = 1500;
        const int maxTotalLength = 50000;

        static string Truncate(string value, int max)
            => value.Length <= max ? value : value[..max] + "...";

        var records = request.Records
            .Select(r =>
            {
                var parts = new[]
                {
                    !string.IsNullOrWhiteSpace(r.Emotion) ? $"e*: {r.Emotion}" : null,
                    !string.IsNullOrWhiteSpace(r.Reason) ? $"r*: {r.Reason}" : null,
                    !string.IsNullOrWhiteSpace(r.Solution) ? $"s*: {r.Solution}" : null,
                    !string.IsNullOrWhiteSpace(r.Intensity) ? $"i*: {r.Intensity}" : null,
                    !string.IsNullOrWhiteSpace(r.Notes) ? $"n*: {r.Notes}" : null,
                }.Where(x => x != null);

                var combined = string.Join("; ", parts);
                return Truncate(combined, maxRecordLength);
            })
            .ToList();

        var userText = string.Join("\n", records);
        if (userText.Length > maxTotalLength)
            userText = userText[..maxTotalLength] + "...";

        var messages = new ChatMessage[]
        {
            new SystemChatMessage(
                $"You are an experienced emotional psychology expert. " +
                $"Analyze the user's emotional records and identify their key triggers, recurring emotional patterns, and provide one realistic, empathetic piece of advice. " +
                $"Focus on emotional depth and psychological accuracy rather than generic self-help. " +
                $"Base your insights strictly on the provided data — do not invent new details. " +
                $"Each record uses compact notation: e* = emotion, r* = reason, s* = solution, i* = intensity, n* = notes " +
                $"Respond in {request.Language} language."
            ),
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