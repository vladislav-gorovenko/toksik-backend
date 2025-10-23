using System.Threading.RateLimiting;

namespace ToksikApp.Extensions;

public static class RateLimiterExtensions
{
    public static void AddRateLimiterWithOptions(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var appUserId = context.Request.Headers["x-app-user-id"].ToString();

                if (string.IsNullOrEmpty(appUserId))
                    return RateLimitPartition.GetNoLimiter(string.Empty);

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: appUserId,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromDays(1),
                        AutoReplenishment = true,
                        QueueLimit = 0
                    });
            });

            options.RejectionStatusCode = 429;
        });
    }
}