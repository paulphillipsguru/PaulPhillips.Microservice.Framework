namespace PaulPhillips.Framework.Feature.Core;
public static class FeatureFactory
{
    public static Dictionary<string, Type> Features { get; set; } = [];
    public static Dictionary<string, Type> Events { get; set; } = [];
}