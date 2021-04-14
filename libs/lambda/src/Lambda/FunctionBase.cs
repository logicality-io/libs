using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Logicality.Lambda
{
    public abstract class FunctionBase<TConfig>
        where TConfig : new()
    {
        protected FunctionBase(
            Action<TConfig, IServiceCollection> configureServices,
            string environmentVariablesPrefix = "")
        {
            var config = new TConfig();
            new ConfigurationBuilder()
                .AddEnvironmentVariables(environmentVariablesPrefix)
                .Build()
                .Bind(config);

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
            });

            configureServices(config, services);

            ServiceProvider = services.BuildServiceProvider();
        }

        protected IServiceProvider ServiceProvider { get; }
    }
}
