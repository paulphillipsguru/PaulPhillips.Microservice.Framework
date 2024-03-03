namespace PaulPhillips.Framework.Feature.Core.Models
{
    public class FeatureRequest
    {
        public Type? Feature { get; set; }
        public string? TraceId { get; set; }
        public string? IdempotencyKey { get;set; }
        public int? IdempotencyHash => IdempotencyKey?.GetHashCode() ?? null;
        public string Body {  get; set; } = string.Empty;   
    }
}
