using Microsoft.Extensions.Configuration;
namespace PaulPhillips.Framework.Feature.Helpers;

public static class ConfigurationHelper
{
    public static string GetConfigValue(this IConfiguration configuration, string name, bool throwIfDoesNotExist = true)
    {
        var value = configuration[name];
        if (value == null && throwIfDoesNotExist) {
            throw new ApplicationException($"Missing required configuration {name}.");
        }

        return value ?? "";
    }

    public static string GetConfigValue(this IConfiguration configuration, string name, string defaultValue)
    {
        var value = configuration[name];            
        return value ?? defaultValue;
    }
}
