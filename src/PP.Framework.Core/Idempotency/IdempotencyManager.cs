using Microsoft.Extensions.Configuration;
using PaulPhillips.Framework.Feature.Idempotency.Contracts;
using PaulPhillips.Framework.Feature.Idempotency.Models;
using PaulPhillips.Framework.Feature.Helpers;
using StackExchange.Redis;
namespace PaulPhillips.Framework.Feature.Idempotency
{
    public class IdempotencyManager(IConfiguration configuration) : IIdempotency
    {
        public async Task<IdempotencyResponeModel> ManageIdempotencyRequest(IdempotencyModel idempotencyModel)
        {
            var idempotencyModelResponse = new IdempotencyResponeModel();
            var requestKey= $"Request_{idempotencyModel.Name}_{idempotencyModel.Id}";
            var responseKey= $"Response_{idempotencyModel.Name}_{idempotencyModel.Id}";
            using var redis = ConnectionMultiplexer.Connect(configuration.GetConfigValue("Idempotency:Host"));
            var db = redis.GetDatabase();

            var hashCode = await db.StringGetAsync(requestKey);
            if (hashCode.HasValue)
            {                
                if (hashCode == idempotencyModel.MessageHash)
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
                
                await db.StringSetAsync(requestKey, idempotencyModel.MessageHash);
            }

            return idempotencyModelResponse;
        }
        public async Task<bool> ManageIdempotencyResponse(IdempotencyModel idempotencyModel)
        {
            using var redis = ConnectionMultiplexer.Connect(configuration.GetConfigValue("Idempotency:Host"));
            var db = redis.GetDatabase();
            var responseKey = $"Response_{idempotencyModel.Name}_{idempotencyModel.Id}";
            await db.StringSetAsync(responseKey, idempotencyModel.Response);            
            return true;
        }
    }
}
