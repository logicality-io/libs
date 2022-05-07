using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Logicality.Lambda;

/// <summary>
/// A base function (using the Event Invocation type), that encapsulates
/// some boiler plate code around configuration, logging and setting up dependency injection 
/// </summary>
/// <typeparam name="TOptions"></typeparam>
/// <typeparam name="THandler"></typeparam>
public abstract class FunctionBase<TOptions, THandler>
    where THandler : class where TOptions : class
{
    private IServiceProvider? _serviceProvider;

    /// <summary>
    /// Initializes an new instance of <see cref="FunctionBase{TOptions,THandler}"/>.
    /// </summary>
    protected FunctionBase()
    {
        var hostConfiguration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
        Environment = hostConfiguration[HostDefaults.EnvironmentKey] ?? Environments.Production;
    }

    /// <summary>
    /// The environment that from environment variables using key <see cref="HostDefaults.EnvironmentKey"/>.
    /// Initialized via constructor.
    /// </summary>
    protected string Environment { get; }

    /// <summary>
    /// Get's the service provider. Invoked each time a lambda is handler is called.
    /// On first call the configuration, logging and service collection are initialized.
    /// Subsequent calls return a cached instance of the service provider.
    /// </summary>
    protected IServiceProvider GetServiceProvider()
    {
        if (_serviceProvider != null)
        {
            return _serviceProvider;
        }
        var configurationBuilder = new ConfigurationBuilder();
        ConfigureConfiguration(configurationBuilder);
        var configurationRoot = configurationBuilder.Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configurationRoot);
        services.AddLogging(logging =>
        {
            logging.AddConfiguration(configurationRoot);
            ConfigureLogging(logging);
        });
        services.AddOptions<TOptions>().Bind(configurationRoot);
        services.AddTransient<THandler>();
        ConfigureServices(services);

        _serviceProvider = services.BuildServiceProvider();

        return _serviceProvider;
    }

    /// <summary>
    /// Configures the configuration builder. Adds appsettings.json, appsettings.{Environment}.json
    /// and environment variables.
    /// </summary>
    /// <param name="configuration">A configuration builder.</param>
    protected virtual void ConfigureConfiguration(IConfigurationBuilder configuration)
    {
        configuration
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment}.json", optional: true)
            .AddEnvironmentVariables();
    }

    /// <summary>
    /// Configure the logging. Adds <see cref="LambdaILoggerProvider"/> provider.
    /// </summary>
    /// <param name="logging">A logging builder.</param>
    protected virtual void ConfigureLogging(ILoggingBuilder logging)
    {
        var loggerOptions = new LambdaLoggerOptions
        {
            IncludeException = true,
            IncludeEventId   = true,
            IncludeScopes    = true
        };
        logging.AddLambdaLogger(loggerOptions);
    }

    /// <summary>
    /// Configure the services.
    /// </summary>
    /// <param name="services">A service collection.</param>
    protected virtual void ConfigureServices(IServiceCollection services)
    { }
}