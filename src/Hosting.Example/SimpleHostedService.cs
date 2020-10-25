using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Logicality.Extensions.Hosting.Example
{
    public class SimpleHostedService : IHostedService
    {
        private readonly ILogger _logger;

        public SimpleHostedService(ExampleContext context, ILogger logger)
        {
            Context = context;
            _logger = logger;
        }

        protected ExampleContext Context { get; }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken);
            _logger.LogInformation("Started");
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopped");
            return Task.CompletedTask;
        }
    }
}
