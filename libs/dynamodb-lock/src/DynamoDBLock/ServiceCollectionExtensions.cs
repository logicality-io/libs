using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logicality.DynamoDBLock;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDynamoDBLockClient(
        this IServiceCollection services,
        IConfiguration          namedConfigurationSection)
    {
        services.AddTransient<DynamoDBLockClient>();
        services.Configure<DynamoDBLockClientOptions>(namedConfigurationSection);
        return services;
    }

    public static IServiceCollection AddDynamoDBLockClient(
        this IServiceCollection           services,
        Action<DynamoDBLockClientOptions> configureOptions)
    {
        services.AddTransient<DynamoDBLockClient>();
        services.Configure(configureOptions);
        return services;
    }

    public static IServiceCollection AddDynamoDBLockClient(
        this IServiceCollection   services,
        DynamoDBLockClientOptions userOptions)
    {
        services.AddTransient<DynamoDBLockClient>();
        services.AddOptions<DynamoDBLockClientOptions>()
            .Configure(options =>
            {
                // Overwrite default option values
                // with the user provided options.
                // options.SomeValue = userOptions.SomeValue;
            });

        return services;
    }
}