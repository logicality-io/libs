using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Logicality.Extensions.Hosting.Example
{
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
            // Write a log file to the bin folder. Very useful to share log files (not just via seq)
            var logFile = "debug.log";
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.Logger(l =>
                {
                    l.WriteHostedServiceMessagesToConsole();
                });

            var logger = loggerConfiguration.CreateLogger();

            var context = new ExampleContext();

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
}
