using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
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
    
    public class AnalyzeEmotionsRequest
    {
        public string Text { get; set; } = string.Empty;
    }

    [HttpPost("analyze-emotions")]
    public async Task<GetAiSummaryResponse> AnalyzeEmotions([FromBody] AnalyzeEmotionsRequest request)
    {
        var messages = new ChatMessage[]
        {
            new SystemChatMessage("You are an emotion analysis expert. Analyze the emotional content of the text and provide a summary of the emotions detected."),
            new UserChatMessage(request.Text)
        };

        var completion = await _chatClient.CompleteChatAsync(messages);
        var response = completion.Value.Content[0].Text;

        return new GetAiSummaryResponse(response);
    }
}