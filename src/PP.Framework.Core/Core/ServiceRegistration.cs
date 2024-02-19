using Microsoft.Extensions.DependencyInjection;
using PaulPhillips.Framework.Feature.Events;
using PaulPhillips.Framework.Feature.Events.Contracts;
using PaulPhillips.Framework.Feature.Idempotency;
using PaulPhillips.Framework.Feature.Tracing;

namespace PaulPhillips.Framework.Feature.Core
{
    public static class ServiceRegistration
    {
        public static void RegisterFeature(this IServiceCollection services)
        {
            services.AddSingleton<FeatureService>();
        }
        public static void RegisterFeatureEventManager(this IServiceCollection services)
        {
            services.AddSingleton<IEventManager, EventManager>();
        }

        public static void RegisterFeatureAll(this IServiceCollection services)
        {
            
            services.RegisterFeature();
            services.RegisterFeatureTracing();
            services.RegisterFeatureEventManager();
            services.RegisterIdempotency();
        }
    }
}
