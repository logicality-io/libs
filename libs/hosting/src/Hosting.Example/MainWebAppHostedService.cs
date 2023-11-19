using Microsoft.AspNetCore;
using Serilog;

namespace Logicality.Extensions.Hosting.Example;

public class MainWebAppHostedService(HostedServiceContext context) : IHostedService
{
    private          IWebHost?            _webHost;
    public const     int                  Port = 5000;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "SeqUri", context.Seq.SinkUri.ToString() }
            })
            .Build();

        _webHost = WebHost
            .CreateDefaultBuilder<Startup>([])
            .UseUrls($"http://+:{Port}")
            .UseConfiguration(config)
            .Build();

        context.MainWebApp = this;

        return _webHost.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) 
        => _webHost!.StopAsync(cancellationToken);

    public class Startup(IConfiguration configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                var seqUri = configuration.GetValue<string>("SeqUri")!;
                var logger = new LoggerConfiguration()
                    .WriteTo.Seq(seqUri)
                    .CreateLogger();
                logging.AddSerilog(logger);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Run(context 
                => context.Response.WriteAsync("Hello MainWebApp"));
        }
    }
}