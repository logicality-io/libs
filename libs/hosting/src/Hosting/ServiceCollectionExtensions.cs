using Logicality.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a set of hosted services that are started in the order they are registered.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="name">A friendly name that represents the collection of hosted services. Used in log output.</param>
    /// <param name="configure">Configure the set of child hosted services.</param>
    /// <returns></returns>
    public static IServiceCollection AddSequentialHostedServices(
        this IServiceCollection                services,
        string                                 name,
        Action<SequentialHostedServiceBuilder> configure)
    {
        var builder = new SequentialHostedServiceBuilder(services, name);
        configure(builder);
        services.AddHostedService(sp => builder.Build(sp));
        return services;
    }

    /// <summary>
    /// Adds a set of hosted services that are started in parallel.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="name">A friendly name that represents the collection of hosted services. Used in log output.</param>
    /// <param name="configure">Configure the set of child hosted services.</param>
    public static IServiceCollection AddParallelHostedServices(
        this IServiceCollection              services,
        string                               name,
        Action<ParallelHostedServiceBuilder> configure)
    {
        var builder = new ParallelHostedServiceBuilder(services, name);
        configure(builder);
        services.AddHostedService(sp => builder.Build(sp));
        return services;
    }
}