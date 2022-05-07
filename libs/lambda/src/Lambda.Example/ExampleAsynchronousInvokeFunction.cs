using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Logicality.Lambda.Example;

public class ExampleAsynchronousInvokeFunction : AsynchronousInvokeFunction<Request, ExampleOptions, ExampleAsynchronousInvokeHandler>
{
    protected override void ConfigureConfiguration(IConfigurationBuilder configuration)
    {
        base.ConfigureConfiguration(configuration);
    }

    protected override void ConfigureLogging(ILoggingBuilder logging)
    {
        base.ConfigureLogging(logging);
        logging.SetMinimumLevel(LogLevel.Debug);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddHttpClient();
    }
}