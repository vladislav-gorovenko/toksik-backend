using System.Net.Http.Headers;
using System.Text.Json;
using ToksikApp.Contracts.GetAiSummary.Responses.Query;

namespace ToksikApp.Middleware;

public class RevenueCatMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _factory;
    private readonly string _apiKey;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public RevenueCatMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _next = next;
        _factory = httpClientFactory;
        _apiKey = configuration["RevenueCat:ApiKey"]!;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var appUserId = context.Request.Headers["x-app-user-id"].ToString();
        if (string.IsNullOrEmpty(appUserId))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Subscription check failed");
            return;
        }

        var client = _factory.CreateClient();
        var url = $"https://api.revenuecat.com/v1/subscribers/{Uri.EscapeDataString(appUserId)}";

        var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var res = await client.SendAsync(req);

        if (!res.IsSuccessStatusCode)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Subscription check failed");
            return;
        }

        var json = await res.Content.ReadAsStringAsync();
        var customerInfo = JsonSerializer.Deserialize<GetRevenueCatCustomerInfoQueryResponse>(json, _jsonOptions);
        var premiumAccessEntitlement =
            customerInfo?.Subscriber?.Entitlements?.GetValueOrDefault("Premium access");

        if (premiumAccessEntitlement is null)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Premium required");
            return;
        }

        if (premiumAccessEntitlement.ExpiresDate.HasValue && premiumAccessEntitlement.ExpiresDate < DateTime.UtcNow)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Premium required");
            return;
        }

        await _next(context);
    }
}