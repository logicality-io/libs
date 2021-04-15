using System.Collections.Generic;
using System.Net.Http;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace Logicality.Lambda.Example
{
    public class ExampleFunction: FunctionBase<FunctionConfig, Handler>
    {
        public ExampleFunction() 
            : base(ConfigureConfiguration, ConfigureLogging, ConfigureServices)
        { }

        public ExampleFunction(string environmentVariables)
            : base(ConfigureConfiguration, ConfigureLogging, ConfigureServices, environmentVariables) 
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

        public string? Handle(string input, ILambdaContext context)
        {
            var handler = ServiceProvider.GetRequiredService<Handler>();
            return handler.Handle(input, context);
        }
    }

    public class FunctionConfig
    {
        public string Foo { get; set; }
    }

    public class Handler
    {
        private readonly IHttpClientFactory _clientFactory;

        public Handler(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public string? Handle(string input, ILambdaContext context)
        {
            return input?.ToUpper();
        }
    }
}
