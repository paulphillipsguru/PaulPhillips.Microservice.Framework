using PaulPhillips.Framework.Feature.Idempotency.Enums;

namespace PaulPhillips.Framework.Feature.Idempotency.Models
{
    public class IdempotencyResponeModel
    {        
        public IdempotencyStatus Status {  get; set; } = IdempotencyStatus.RefreshResponse;
        public dynamic? Response { get; set; }
    }
}
