using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace PaulPhillips.Framework.Feature.Middlewares
{
    public static class FeatureCoreRegisterAllMiddleware
    {
        public static void UseFeatureFramework(this WebApplication app)
        {

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                }
            });
            app.UseMiddleware<FeatureSecurityMiddleware>();
            app.UseMiddleware<FeaterCoreMiddleware>();
        }
    }
}
