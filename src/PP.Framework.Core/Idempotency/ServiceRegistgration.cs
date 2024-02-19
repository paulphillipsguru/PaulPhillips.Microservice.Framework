using Microsoft.Extensions.DependencyInjection;
using PaulPhillips.Framework.Feature.Idempotency.Contracts;

namespace PaulPhillips.Framework.Feature.Idempotency
{
    public static class ServiceRegistgration
    {
        public static void RegisterIdempotency(this IServiceCollection services)
        {
            services.AddSingleton<IIdempotency, IdempotencyManager>();
        }
    }
}
