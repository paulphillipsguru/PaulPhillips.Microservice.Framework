using PaulPhillips.Framework.Feature.Core.Models;
using PaulPhillips.Framework.Feature.Idempotency.Models;

namespace PaulPhillips.Framework.Feature.Idempotency.Contracts
{
    public interface IIdempotency
    {
        Task<IdempotencyResponeModel> ManageIdempotencyRequest(FeatureRequest featureRequest);
        Task<bool> ManageIdempotencyResponse(FeatureRequest featureRequest, dynamic response);
    }
}
