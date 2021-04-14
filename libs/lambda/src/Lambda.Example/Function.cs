using System.Net.Http;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace Logicality.Lambda.Example
{
    public class Function: FunctionBase<FunctionConfig>
    {
        public Function() : base(ConfigureServices)
        { }

        private static void ConfigureServices(FunctionConfig config, IServiceCollection services)
        {
            services.AddSingleton(config);
            services.AddHttpClient();
            services.AddTransient<Handler>();
        }

        public string? FunctionHandler(string input, ILambdaContext context)
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
        private readonly HttpClient _client;

        public Handler(HttpClient client)
        {
            _client = client;
        }

        public string? Handle(string input, ILambdaContext context)
        {
            return input?.ToUpper();
        }
    }
}
