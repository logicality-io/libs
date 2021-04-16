using System;
using System.Collections.Generic;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Logicality.Lambda.Example
{
    public class ExampleFunction: FunctionBase<FunctionConfig, Handler>
    {
        // This constructor will be called by Lambda runtime.
        public ExampleFunction() 
            : base(ConfigureConfiguration, ConfigureLogging, ConfigureServices)
        { }

        // This constuctor is to support tests.
        public ExampleFunction(Action<IConfigurationBuilder> configureConfiguration)
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

        public string? Handle(string input, ILambdaContext context)
        {
            var handler = ServiceProvider.GetRequiredService<Handler>();
            return handler.Handle(input, context);
        }
    }
}