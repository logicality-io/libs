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
                    services.AddTransient<Simple1HostedService>();
                    services.AddTransient<Simple2HostedService>();
                    services.AddTransient<Simple3HostedService>();
                    services.AddTransient<Simple4HostedService>();
                    services.AddTransient<Simple5HostedService>();
                    services.AddTransient<SeqHostedService>();

                    services.AddSequentialHostedServices("Sequential", 
                        s1 => s1
                            .HostSequential("Sequential2", 
                                s2 => s2
                                    .Host<Simple3HostedService>()
                                    .Host<Simple4HostedService>())
                            .HostParallel("parallel1", 
                                p1 => p1
                                    .Host<Simple2HostedService>()
                                    .Host<Simple5HostedService>()));
                })
                .UseSerilog(logger);
        }
    }
}
