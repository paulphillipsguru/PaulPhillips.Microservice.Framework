using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PaulPhillips.Framework.Feature.Security;

namespace PaulPhillips.Framework.Feature.Middlewares
{
    public class FeatureSecurityMiddleware(IConfiguration configuration, RequestDelegate next)
    {        
        private const int StatusUnAuthorised = 401;
        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.ToString().StartsWith("/health", StringComparison.CurrentCultureIgnoreCase))
            {
                var configSecretKey = configuration["Security:Key"] ?? string.Empty;
                var configIssuer = configuration["Security:Issuer"] ?? string.Empty;
                var configAudience = configuration["Security:Audience"] ?? string.Empty;

                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    string? token = context.Request.Headers.Authorization;
                    if (token != null)
                    {
                        var canAccessFeature = JwtHelper.ValidateToken(token, configSecretKey, configIssuer, configAudience);
                        if (!canAccessFeature.Item1)
                        {
                            context.Response.StatusCode = StatusUnAuthorised;
                        } else
                        {
                            await next(context);
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusUnAuthorised;
                }
            }
            else
            {
                await next(context);
            }
        }
    }
}
