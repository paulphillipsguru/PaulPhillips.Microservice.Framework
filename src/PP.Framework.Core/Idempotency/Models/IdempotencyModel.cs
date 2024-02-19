namespace PaulPhillips.Framework.Feature.Idempotency.Models
{
    public class IdempotencyModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int MessageHash { get; set; } = 0;        
        public string? Response { get; set; }
    }
}
