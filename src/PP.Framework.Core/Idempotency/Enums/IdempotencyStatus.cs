namespace PaulPhillips.Framework.Feature.Idempotency.Enums
{
    public enum IdempotencyStatus
    {
        UseCachedResponse,
        RefreshResponse,
        RequestFoundButHashIsDifferent
    }
}
