using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Logicality.Lambda.Example
{
    public class ExampleSynchronousFunction: SynchronousFunctionBase<FunctionConfig, string, string, SynchronousHandler>
    {
        // This constructor will be called by Lambda runtime.
        public ExampleSynchronousFunction() 
            : base(ConfigureConfiguration, ConfigureLogging, ConfigureServices)
        { }

        // This constuctor is to support tests.
        public ExampleSynchronousFunction(Action<IConfigurationBuilder> configureConfiguration)
            : base(configuration =>
            {
                ConfigureConfiguration(configuration);
                configureConfiguration(configuration);
            }, ConfigureLogging, ConfigureServices)
        {}

        private static void ConfigureConfiguration(IConfigurationBuilder configuration)
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"foo", "bar"}
            });
        }

        private static void ConfigureLogging(ILoggingBuilder logging)
        {
            logging.SetMinimumLevel(LogLevel.Debug);
        }

        private static void ConfigureServices(FunctionConfig config, IServiceCollection services)
        {
            services.AddHttpClient();
        }
    }

    public class ExampleAsynchronousFunction : AsynchronousFunctionBase<FunctionConfig, string, AsynchronousHandler>
    {
        // This constructor will be called by Lambda runtime.
        public ExampleAsynchronousFunction()
            : base(ConfigureConfiguration, ConfigureLogging, ConfigureServices)
        { }

        // This constuctor is to support tests.
        public ExampleAsynchronousFunction(Action<IConfigurationBuilder> configureConfiguration)
            : base(configuration =>
            {
                ConfigureConfiguration(configuration);
                configureConfiguration(configuration);
            }, ConfigureLogging, ConfigureServices)
        { }

        private static void ConfigureConfiguration(IConfigurationBuilder configuration)
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"foo", "bar"}
            });
        }

        private static void ConfigureLogging(ILoggingBuilder logging)
        {
            logging.SetMinimumLevel(LogLevel.Debug);
        }

        private static void ConfigureServices(FunctionConfig config, IServiceCollection services)
        {
            services.AddHttpClient();
        }
    }
}