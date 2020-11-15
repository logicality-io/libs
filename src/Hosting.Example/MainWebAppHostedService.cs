using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Logicality.Extensions.Hosting.Example
{
    public class MainWebAppHostedService : IHostedService
    {
        private readonly ExampleContext _context;
        private IWebHost? _webHost;
        public const int Port = 5000;

        public MainWebAppHostedService(ExampleContext context)
        {
            _context = context;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "SeqUri", _context.Seq.SinkUri.ToString() }
                })
                .Build();

            _webHost = WebHost
                .CreateDefaultBuilder<Startup>(Array.Empty<string>())
                .UseUrls($"http://+:{Port}")
                .UseConfiguration(config)
                .Build();

            _context.MainWebApp = this;

            return _webHost.StartAsync(cancellationToken);
        }

        public Task? StopAsync(CancellationToken cancellationToken) 
            => _webHost?.StopAsync(cancellationToken);

        public class Startup
        {
            private readonly IConfiguration _configuration;

            public Startup(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddLogging(logging =>
                {
                    var seqUri = _configuration.GetValue<string>("SeqUri");
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
}
