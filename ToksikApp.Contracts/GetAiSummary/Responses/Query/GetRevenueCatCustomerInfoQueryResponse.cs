using System.Text.Json.Serialization;

namespace ToksikApp.Contracts.GetAiSummary.Responses.Query;

public class GetRevenueCatCustomerInfoQueryResponse
{
    [JsonPropertyName("request_date")] public DateTime RequestDate { get; set; }

    [JsonPropertyName("request_date_ms")] public long RequestDateMs { get; set; }

    [JsonPropertyName("subscriber")] public Subscriber Subscriber { get; set; } = null!;
}

public class Subscriber
{
    [JsonPropertyName("entitlements")] public Dictionary<string, Entitlement> Entitlements { get; set; } = new();

    [JsonPropertyName("first_seen")] public DateTime FirstSeen { get; set; }

    [JsonPropertyName("last_seen")] public DateTime? LastSeen { get; set; }

    [JsonPropertyName("management_url")] public string? ManagementUrl { get; set; }

    [JsonPropertyName("non_subscriptions")]
    public Dictionary<string, List<NonSubscription>> NonSubscriptions { get; set; } = new();

    [JsonPropertyName("original_app_user_id")]
    public string? OriginalAppUserId { get; set; }

    [JsonPropertyName("original_application_version")]
    public string? OriginalApplicationVersion { get; set; }

    [JsonPropertyName("original_purchase_date")]
    public DateTime? OriginalPurchaseDate { get; set; }

    [JsonPropertyName("other_purchases")] public Dictionary<string, object> OtherPurchases { get; set; } = new();

    [JsonPropertyName("subscriptions")] public Dictionary<string, Subscription> Subscriptions { get; set; } = new();
}

public class Entitlement
{
    [JsonPropertyName("expires_date")] public DateTime? ExpiresDate { get; set; }

    [JsonPropertyName("grace_period_expires_date")]
    public DateTime? GracePeriodExpiresDate { get; set; }

    [JsonPropertyName("product_identifier")]
    public string? ProductIdentifier { get; set; }

    [JsonPropertyName("purchase_date")] public DateTime? PurchaseDate { get; set; }
}

public class Subscription
{
    [JsonPropertyName("auto_resume_date")] public DateTime? AutoResumeDate { get; set; }

    [JsonPropertyName("billing_issues_detected_at")]
    public DateTime? BillingIssuesDetectedAt { get; set; }

    [JsonPropertyName("display_name")] public string? DisplayName { get; set; }

    [JsonPropertyName("expires_date")] public DateTime? ExpiresDate { get; set; }

    [JsonPropertyName("grace_period_expires_date")]
    public DateTime? GracePeriodExpiresDate { get; set; }

    [JsonPropertyName("is_sandbox")] public bool IsSandbox { get; set; }

    [JsonPropertyName("management_url")] public string? ManagementUrl { get; set; }

    [JsonPropertyName("original_purchase_date")]
    public DateTime? OriginalPurchaseDate { get; set; }

    [JsonPropertyName("ownership_type")] public string? OwnershipType { get; set; }

    [JsonPropertyName("period_type")] public string? PeriodType { get; set; }

    [JsonPropertyName("price")] public Price? Price { get; set; }

    [JsonPropertyName("purchase_date")] public DateTime? PurchaseDate { get; set; }

    [JsonPropertyName("refunded_at")] public DateTime? RefundedAt { get; set; }

    [JsonPropertyName("store")] public string? Store { get; set; }

    [JsonPropertyName("store_transaction_id")]
    public string? StoreTransactionId { get; set; }

    [JsonPropertyName("unsubscribe_detected_at")]
    public DateTime? UnsubscribeDetectedAt { get; set; }
}

public class Price
{
    [JsonPropertyName("amount")] public decimal Amount { get; set; }

    [JsonPropertyName("currency")] public string? Currency { get; set; }
}

public class NonSubscription
{
    [JsonPropertyName("id")] public string? Id { get; set; }

    [JsonPropertyName("is_sandbox")] public bool IsSandbox { get; set; }

    [JsonPropertyName("purchase_date")] public DateTime? PurchaseDate { get; set; }

    [JsonPropertyName("store")] public string? Store { get; set; }
}