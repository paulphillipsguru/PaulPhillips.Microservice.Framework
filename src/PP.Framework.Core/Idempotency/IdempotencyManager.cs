using Microsoft.Extensions.Configuration;
using PaulPhillips.Framework.Feature.Core.Models;
using PaulPhillips.Framework.Feature.Helpers;
using PaulPhillips.Framework.Feature.Idempotency.Contracts;
using PaulPhillips.Framework.Feature.Idempotency.Models;
using StackExchange.Redis;
namespace PaulPhillips.Framework.Feature.Idempotency
{
    public class IdempotencyManager(IConfiguration configuration) : IIdempotency
    {
        public async Task<IdempotencyResponeModel> ManageIdempotencyRequest(FeatureRequest featureRequest)
        {
            ArgumentNullException.ThrowIfNull(nameof(featureRequest));  
            ArgumentNullException.ThrowIfNull(nameof(featureRequest.Feature));

            var featureName = featureRequest?.Feature.Name;
            var idempotencyModelResponse = new IdempotencyResponeModel();
            var requestKey= $"Request_{featureName}_{featureRequest.IdempotencyKey}";
            var responseKey= $"Response_{featureName}_{featureRequest.IdempotencyKey}";
            using var redis = ConnectionMultiplexer.Connect(configuration.GetConfigValue("Idempotency:Host"));
            var db = redis.GetDatabase();

            var hashCode = await db.StringGetAsync(requestKey);
            if (hashCode.HasValue)
            {                
                if (hashCode == featureRequest.IdempotencyHash)
                {
                    var response = await db.StringGetAsync(responseKey);
                    if (response.HasValue)
                    {
                        idempotencyModelResponse.Response = response;
                        idempotencyModelResponse.Status = Enums.IdempotencyStatus.UseCachedResponse;
                    }
                    else
                    {
                        idempotencyModelResponse.Status = Enums.IdempotencyStatus.RefreshResponse;
                    }
                }
                else
                {
                    idempotencyModelResponse.Status = Enums.IdempotencyStatus.RequestFoundButHashIsDifferent;
                }
            }
            else
            {
                
                await db.StringSetAsync(requestKey, featureRequest.IdempotencyHash);
            }

            return idempotencyModelResponse;
        }
        public async Task<bool> ManageIdempotencyResponse(FeatureRequest featureRequest,dynamic response)
        {
            ArgumentNullException.ThrowIfNull(nameof(featureRequest));
            ArgumentNullException.ThrowIfNull(nameof(response));

            using var redis = ConnectionMultiplexer.Connect(configuration.GetConfigValue("Idempotency:Host"));
            var db = redis.GetDatabase();

            var featureName = featureRequest?.Feature?.Name;
            var responseKey = $"Response_{featureName}_{featureRequest?.IdempotencyKey}";
            await db.StringSetAsync(responseKey, response);            
            return true;
        }
    }
}
