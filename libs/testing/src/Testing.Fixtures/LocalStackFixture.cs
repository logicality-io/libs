using System;
using System.Linq;
using System.Threading.Tasks;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Common;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Polly;

namespace Logicality.Testing.Fixtures;

public class LocalstackFixture : IDisposable
{
    private const    int               ContainerPort = 4566;
    private readonly IContainerService _containerService;

    private LocalstackFixture(IContainerService containerService, Uri serviceUrl)
    {
        _containerService = containerService;
        ServiceUrl        = serviceUrl;
    }

    /// <summary>
    /// Create an instance of <see cref="LocalstackFixture"/>
    /// </summary>
    /// <param name="containerNamePrefix">
    ///     A prefix for the container name.
    ///     Use to isolate between multiple local stack instances running concurrently.
    /// </param>
    /// <param name="services">The collection os services to run. See LocalStack documentation.</param>
    /// <param name="imageTag">LocalStack image tag. Default is 'latest'.</param>
    /// <param name="proApiKey">LocalStack Pro API key</param>
    /// <param name="port">
    ///     Default is '0' which will bind to a random free port. Useful when
    ///     running multiple container instances and not having port conflicts on the host.
    /// </param>
    /// <returns></returns>
    public static Task<LocalstackFixture> Create(
        string containerNamePrefix,
        string services,
        string imageTag  = "latest",
        string proApiKey = "",
        int    port      = 0)
    {
        return Task.Run(() =>
        {
            IContainerService containerService;
            var               name = $"{containerNamePrefix}-localstack";
            try
            {
                containerService = new Builder()
                    .UseContainer()
                    .WithName(name)
                    .UseImage($"localstack/localstack:{imageTag}")
                    .ReuseIfExists()
                    .ExposePort(port, ContainerPort)
                    .WithEnvironment("LS_LOG=debug")
                    .WithEnvironment($"LOCALSTACK_API_KEY={proApiKey}")
                    .WithEnvironment($"SERVICES={services}")
                    .WaitForPort($"{ContainerPort}/tcp", TimeSpan.FromSeconds(10))
                    .Build();
                containerService.Start();
            }
            catch (FluentDockerException ex) when (ex.Message.Contains("Error response from daemon: Conflict"))
            {
                // This can happen in a container startup race condition and parallel tests.
                // Assume the container is already running.

                var hosts  = new Hosts().Discover();
                var docker = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");
                    
                var waitAndRetry = Policy.Handle<FluentDockerException>()
                    .WaitAndRetry(30, _ => TimeSpan.FromMilliseconds(1000));

                containerService = waitAndRetry.Execute(() =>
                {
                    var containers = docker!.GetContainers();
                    var container  = containers.Single(c => c.Name == name);
                    container.WaitForPort($"{ContainerPort}/tcp", 5000);
                    return container;
                });
            }

            var config          = containerService.GetConfiguration();
            var networkSettings = config.NetworkSettings;
            var exposedPort     = networkSettings.Ports.First();
            var hostPort        = exposedPort.Value.First().HostPort;

            var serviceUrl = new UriBuilder($"http://localhost:{hostPort}");

            if (FixtureUtils.IsRunningInContainer)
            {
                // When tests are running in container, the networking setup is different.
                // Instead of host -> container, we have container -> container so
                // localhost won't work as host networking does not apply
                var host = containerService
                    .GetConfiguration()
                    .NetworkSettings
                    .IPAddress;

                serviceUrl.Host = host;
                serviceUrl.Port = ContainerPort;
            }

            return new LocalstackFixture(containerService, serviceUrl.Uri);
        });
    }

    public Uri ServiceUrl { get; }

    /// <summary>
    /// Disposes and shuts down the container.
    /// </summary>
    public void Dispose()
    {
        _containerService.Dispose();
    }
}