using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Logicality.Lambda
{
    /// <summary>
    /// A base function that encapsulates some boiler plate code around configuration, logging
    /// and setting up dependency injection.
    ///
    /// By default configuration is loaded from environment variables, appsettings.json and 
    /// </summary>
    /// <typeparam name="TConfig">The configuration object that configuration will be bound to.</typeparam>
    /// <typeparam name="THandler">The configuration object that configuration will be bound to.</typeparam>
    public abstract class FunctionBase<TConfig, THandler>
        where TConfig : class, new()
        where THandler: class
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FunctionBase{TConfig, THandler}"/>
        /// </summary>
        /// <param name="configureConfiguration">
        ///     An action to configure the configuration. By default appsettings.json,
        ///     appsettings.{environment}.json and environment variables providers are added.
        /// </param>
        /// <param name="configurelogging">
        ///     An acction to configure logging services. By default the Lambda Logger provider
        ///     is added and the minimum logging level isset to 'Information'.
        /// </param>
        /// <param name="configureServices">
        ///     Configure any services needed for injection into your handler. By default,
        ///     an instance of TConfig is added as a singleton and THandler is added
        ///     as transient.
        /// </param>
        /// <param name="environmentVariablesPrefix">
        ///     A prefix to use for environment variables. Defaults to empty string.
        /// </param>
        protected FunctionBase(
            Action<IConfigurationBuilder>? configureConfiguration = null,
            Action<ILoggingBuilder>? configurelogging = null,
            Action<TConfig, IServiceCollection>? configureServices = null,
            string environmentVariablesPrefix = "")
        {
            configureConfiguration ??= _ => { };
            configureServices ??= (_, _) => { };
            configurelogging ??= _ => { };

            var hostConfiguration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            var environment = hostConfiguration[HostDefaults.EnvironmentKey] ?? Environments.Production;

            var appConfiguration = new TConfig();
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables(environmentVariablesPrefix);

            configureConfiguration(configurationBuilder);

            configurationBuilder
                .Build()
                .Bind(appConfiguration);

            var services = new ServiceCollection();
            services.AddLogging(logging =>
            {
                var loggerOptions = new LambdaLoggerOptions
                {
                    IncludeException = true,
                    IncludeEventId = true,
                    IncludeScopes = true
                };
                logging.AddLambdaLogger(loggerOptions);
                logging.SetMinimumLevel(LogLevel.Information);
                configurelogging(logging);
            });
            services.AddSingleton(appConfiguration);
            services.AddTransient<THandler>();

            configureServices(appConfiguration, services);

            ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Gets the Service Provider.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }
    }
}
