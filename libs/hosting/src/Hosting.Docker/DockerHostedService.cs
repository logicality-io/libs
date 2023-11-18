using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Logicality.Extensions.Hosting;

/// <summary>
/// An <see cref="IHostedService"/> specifically designed to start a docker container. It first checks to see if the
/// container is already running by name. If so it will re-use that instance otherwise it will start
/// a new one. Starting a new one will be done on a background task, so combining this with <see cref="ParallelHostedServices"/>
/// can result in better performing startup experience.
/// </summary>
public abstract class DockerHostedService : IHostedService
{
    private readonly ILogger<DockerHostedService> _logger;
    private readonly bool                         _leaveRunning;
    private          IContainerService?           _containerService;

    /// <summary>
    ///     Constructs a hosted service for running docker containers.
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="leaveRunning">
    ///     Leaves the container running after the hosted service stops. Useful if restarting the host
    ///     regularly and it's ok to keep data between runs (example logging sink). It is up to developer to
    ///     shut 
    /// </param>
    protected DockerHostedService(ILogger<DockerHostedService> logger, bool leaveRunning = false)
    {
        _logger       = logger;
        _leaveRunning = leaveRunning;
    }

    /// <summary>
    ///     The container name. It should be unique with respect to the other containers running
    ///     in the host.
    /// </summary>
    protected abstract string ContainerName { get; }

    protected abstract IContainerService CreateContainerService();

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        var task = Task.Run(() =>
        {
            var result = IsContainerRunning(ContainerName);

            if (result.IsRunning)
            {
                _logger.LogInformation($"Container {ContainerName} already running and is being reused.");
                _containerService = result.ContainerService;
            }
            else
            {
                _logger.LogInformation($"Starting new container {ContainerName}. This may take a few moments if the image is not in cache...");
                _containerService = CreateContainerService();
                _containerService.Start();
            }
        }, cancellationToken);

        return task;
    }

    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_leaveRunning)
        {
            _containerService?.Stop();
            _containerService?.Remove(true);
        }
        return Task.CompletedTask;
    }

    private static (bool IsRunning, IContainerService? ContainerService) IsContainerRunning(string name)
    {
        var runningContainers = GetRunningContainers();
        var containerService  = runningContainers.SingleOrDefault(c => c.Name == name);
        return (containerService != null, containerService);
    }

    private static IList<IContainerService> GetRunningContainers() =>
        new Hosts()
            .Discover()
            .First(x => x.Host != null)
            .GetRunningContainers();
}