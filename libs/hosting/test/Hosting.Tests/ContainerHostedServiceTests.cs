using System;
using System.Threading;
using System.Threading.Tasks;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Logicality.SystemExtensions.Net.Sockets;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Logicality.Extensions.Hosting;

public class ContainerHostedServiceTests
{
    private readonly ILoggerFactory _loggerFactory;

    public ContainerHostedServiceTests(ITestOutputHelper outputHelper)
    {
        _loggerFactory = new LoggerFactory().AddXUnit(outputHelper);
    }

    [Fact]
    public async Task Can_start_and_start_container()
    {
        var hostedService = new HelloWorldHostedService(_loggerFactory.CreateLogger<DockerHostedService>());

        await hostedService.StartAsync(CancellationToken.None);

        await hostedService.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Should_use_existing_container()
    {
        var hostedService1 = new HelloWorldHostedService(_loggerFactory.CreateLogger<DockerHostedService>());
        await hostedService1.StartAsync(CancellationToken.None);

        var hostedService2 = new HelloWorldHostedService(_loggerFactory.CreateLogger<DockerHostedService>());
        await hostedService2.StartAsync(CancellationToken.None);

        await hostedService1.StopAsync(CancellationToken.None);
        await hostedService2.StopAsync(CancellationToken.None);
    }

    public class HelloWorldHostedService : DockerHostedService
    {
        public HelloWorldHostedService(ILogger<DockerHostedService> logger) 
            : base(logger)
        { }

        protected override string ContainerName { get; } = $"hello-world-{Guid.NewGuid():N}";

        protected override IContainerService CreateContainerService()
        {
            var port = PortFinder.GetNext();

            return new Builder()
                .UseContainer()
                .WithName(ContainerName)
                .UseImage("hello-world")
                .ReuseIfExists()
                .ExposePort(port, 80)
                .WithEnvironment("ACCEPT_EULA=Y")
                .Build();
        }
    }
}