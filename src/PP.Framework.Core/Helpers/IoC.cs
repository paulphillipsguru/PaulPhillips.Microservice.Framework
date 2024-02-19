using Autofac;

namespace PaulPhillips.Framework.Feature.Helpers;
public static class IoC
{
    private static readonly ContainerBuilder containerBuilder = new();
    public static IContainer? Container { get; private set; }

    public static void Register<I, C>()
    {
        containerBuilder.RegisterType(typeof(C)).As(typeof(I));
    }
    public static T? CreateInstance<T>()
    {
        if (Container != null)
        {
            return (T)Container.Resolve(typeof(T));
        }
        else
        {
            return default;
        }
    }
    public static void Build()
    {
        Container = containerBuilder.Build();
    }

}