using System.Threading;
using System.Threading.Tasks;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Logging;

namespace Logicality.Extensions.Hosting.Example
{
    public class SeqHostedService : DockerHostedService
    {
        private readonly ExampleContext _context;
        public const int Port = 5010;
        private const int ContainerPort = 80;

        public SeqHostedService(
            ExampleContext context,
            ILogger<DockerHostedService> logger)
            : base(logger)
        {
            _context = context;
        }

        protected override string ContainerName => "extensions-seq";

        protected override IContainerService CreateContainerService()
            => new Builder()
                .UseContainer()
                .WithName(ContainerName)
                .UseImage("datalust/seq:5.1.3000")
                .ReuseIfExists()
                .ExposePort(Port, ContainerPort)
                .WithEnvironment("ACCEPT_EULA=Y")
                .WaitForPort($"{ContainerPort}/tcp", 5000, "127.0.0.1")
                .Build();

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            _context.Seq = this;
        }
    }
}
