using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace Logicality.Extensions.Hosting.Example;

public class SeqHostedService(
    HostedServiceContext         context,
    ILogger<DockerHostedService> logger)
    : DockerHostedService(logger)
{
    private const    int                  Port          = 5010;
    private const    int                  ContainerPort = 80;

    protected override string ContainerName => "extensions-seq";

    public Uri SinkUri { get; } = new($"http://localhost:{Port}");

    protected override IContainerService CreateContainerService()
        => new Builder()
            .UseContainer()
            .WithName(ContainerName)
            .UseImage("datalust/seq:latest")
            .ReuseIfExists()
            .ExposePort(Port, ContainerPort)
            .WithEnvironment("ACCEPT_EULA=Y")
            .WaitForPort($"{ContainerPort}/tcp", 5000, "127.0.0.1")
            .Build();

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        context.Seq = this;
    }
}