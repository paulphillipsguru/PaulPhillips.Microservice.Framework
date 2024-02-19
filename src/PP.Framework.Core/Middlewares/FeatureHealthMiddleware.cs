using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PaulPhillips.Framework.Feature.Core;
using PaulPhillips.Framework.Feature.HealthCheck.Models;

namespace PaulPhillips.Framework.Feature.Middlewares
{
    public class FeatureHealthMiddleware
    {
        private readonly string ConfigHealthAlive = "Health/Alive";
        private readonly string ConfigHealthReady = "Health/Ready";
        private readonly RequestDelegate _next;
        public FeatureHealthMiddleware(IConfiguration configuration, RequestDelegate next)
        {
            ConfigHealthAlive = configuration["Health:Alive"] ?? ConfigHealthAlive;
            ConfigHealthReady = configuration["Health:Ready"] ?? ConfigHealthReady;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == $"/{ConfigHealthReady}")
            {
                context.Response.StatusCode = Health.IsReady ? FeatureServiceStatus.StatusOK : FeatureServiceStatus.StatusServerError;
                await Task.CompletedTask;
            }
            else if (context.Request.Path == $"/{ConfigHealthAlive}")
            {
                context.Response.StatusCode = Health.IsHealthly ? FeatureServiceStatus.StatusOK : FeatureServiceStatus.StatusServerError;

                await Task.CompletedTask;
            }
            else
            {
                await _next(context);
            }
        }
    }
}
