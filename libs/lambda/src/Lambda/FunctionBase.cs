using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace Logicality.Lambda
{
    public abstract class FunctionBase<TOptions, THandler>
       where THandler : class where TOptions : class
    {
        protected FunctionBase(
            Action<IConfigurationBuilder>? configureConfiguration = null,
            Action<ILoggingBuilder>? configureLogging = null,
            Action<IServiceCollection>? configureServices = null,
            string environmentVariablesPrefix = "")
        {
            configureConfiguration ??= _ => { };
            configureServices ??= _ => { };
            configureLogging ??= _ => { };

            var hostConfiguration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            var environment = hostConfiguration[HostDefaults.EnvironmentKey] ?? Environments.Production;

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables(environmentVariablesPrefix);

            configureConfiguration(configurationBuilder);

            var configurationRoot = configurationBuilder.Build();

            var services = new ServiceCollection();
            services.AddLogging(logging =>
            {
                logging.AddConfiguration(configurationRoot);
                var loggerOptions = new LambdaLoggerOptions
                {
                    IncludeException = true,
                    IncludeEventId = true,
                    IncludeScopes = true
                };
                logging.AddLambdaLogger(loggerOptions);
                configureLogging(logging);
            });
            services.AddSingleton<IConfiguration>(configurationRoot);
            services.AddOptions<TOptions>().Bind(configurationRoot);
            services.AddTransient<THandler>();
            services.AddFeatureManagement();

            configureServices(services);

            ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Gets the Service Provider.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }
    }
}