using PaulPhillips.Framework.Feature.Idempotency.Models;

namespace PaulPhillips.Framework.Feature.Idempotency.Contracts
{
    public interface IIdempotency
    {
        Task<IdempotencyResponeModel> ManageIdempotencyRequest(IdempotencyModel idempotencyModel);
        Task<bool> ManageIdempotencyResponse(IdempotencyModel idempotencyModel);
    }
}
