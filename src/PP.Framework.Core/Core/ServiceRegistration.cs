using Microsoft.Extensions.DependencyInjection;
using PaulPhillips.Framework.Feature.Core.Contracts;
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

        public static void RegisterFeatureHealthCheck(this IServiceCollection services)
        {
            services.AddHealthChecks();
        }

        public static void RegisterSagaSupport(this IServiceCollection services)
        {
            services.AddTransient<ISagaSupport, SagaSupport>();
        }

        public static void RegisterFeatureAll(this IServiceCollection services)
        {
            services.RegisterFeatureHealthCheck();
            services.RegisterFeature();
            services.RegisterFeatureTracing();
            services.RegisterFeatureEventManager();
            services.RegisterIdempotency();
            services.RegisterSagaSupport();
        }
    }
}
