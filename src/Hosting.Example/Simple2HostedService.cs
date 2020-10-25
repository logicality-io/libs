using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Logicality.Extensions.Hosting.Example
{
    public class Simple2HostedService : SimpleHostedService
    {
        public Simple2HostedService(ExampleContext context, ILogger<Simple1HostedService> logger)
            : base(context, logger)
        { }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            Context.Simple2 = this;
        }
    }
}