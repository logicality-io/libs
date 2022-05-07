using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Logicality.Extensions.Hosting.Example;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await CreateHostBuilder(args)
            .Build()
            .RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .Enrich.FromLogContext()
            .WriteTo.Logger(l =>
            {
                l.WriteHostedServiceMessagesToConsole();
            });

        var logger = loggerConfiguration.CreateLogger();

        var context = new HostedServiceContext();

        return new HostBuilder()
            .UseConsoleLifetime()
            .ConfigureServices(services =>
            {
                services.AddSingleton(context);
                services.AddTransient<SeqHostedService>();

                services.AddSequentialHostedServices("root", r => r
                    .Host<SeqHostedService>()
                    .Host<MySqlHostedService>()
                    .HostParallel("web-apps", 
                        p => p
                            .Host<MainWebAppHostedService>()
                            .Host<AdminWebAppHostedService>()));
            })
            .UseSerilog(logger);
    }
}