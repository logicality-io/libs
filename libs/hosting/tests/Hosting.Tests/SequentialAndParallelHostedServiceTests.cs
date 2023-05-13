using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Logicality.Extensions.Hosting;

public class SequentialAndParallelHostedServiceTests
{
    private readonly ITestOutputHelper _outputHelper;

    public SequentialAndParallelHostedServiceTests(ITestOutputHelper outputHelper) 
        => _outputHelper = outputHelper;

    [Fact]
    public async Task Can_register_sequential_and_parallel_hosted_services()
    {
        var context = new Context();

        var services = new ServiceCollection();
        services.AddSingleton(context);
        services.AddLogging(configure => configure.AddXUnit(_outputHelper));
        // A bit convoluted but it covers all the registration paths.
        services.AddSequentialHostedServices("sequential",
            s => s
                .Host<ExampleHostedService>()
                .HostSequential("sequential-2",
                    s2 => s2
                        .Host<ExampleHostedService>()
                        .Host<ExampleHostedService>())
                .HostParallel("parallel",
                    p => p
                        .HostParallel("parallel-2",
                            p2 => p2
                                .Host<ExampleHostedService>())
                        .HostSequential("sequential-3", 
                            s3 => s3
                                .Host<ExampleHostedService>()
                                .Host<ExampleHostedService>())));
        services.AddParallelHostedServices("parallel-4",
            p4 => p4.Host<ExampleHostedService>());

        var serviceProvider = services.BuildServiceProvider();

        var hostedServices = serviceProvider
            .GetServices<IHostedService>()
            .ToArray();

        foreach (var hostedService in hostedServices)
        {
            await hostedService.StartAsync(CancellationToken.None);
        }

        foreach (var hostedService in hostedServices)
        {
            await hostedService.StopAsync(CancellationToken.None);
        }

        context.Counter.ShouldBe(6);
    }
}