using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Logicality.Lambda
{
    public abstract class FunctionBase<TConfig, THandler>
        where TConfig : class, new()
        where THandler : class
    {
        protected FunctionBase(
            Action<IConfigurationBuilder>? configureConfiguration = null,
            Action<ILoggingBuilder>? configureLogging = null,
            Action<TConfig, IServiceCollection>? configureServices = null,
            string environmentVariablesPrefix = "")
        {
            configureConfiguration ??= _ => { };
            configureServices ??= (_, _) => { };
            configureLogging ??= _ => { };

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
                configureLogging(logging);
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