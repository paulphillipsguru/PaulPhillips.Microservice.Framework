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
            bool.TryParse(configuration["Security:Enabled"], out var securityEnabled);

            if (securityEnabled)
            {
                var configSecretKey = configuration["Security:Key"];
                var configIssuer = configuration["Security:Issuer"];
                var configAudience = configuration["Security:Audience"];

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
