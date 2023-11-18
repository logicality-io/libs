using Microsoft.Extensions.Hosting;

namespace Logicality.Extensions.Hosting;

/// <summary>
/// An <see cref="IHostedService"/> that contains child hosted services and starts
/// them in parallel. Useful for a set of hosted services that are not dependent on
/// each other resulting in higher performing startup (on multi-core machines).
/// </summary>
public class ParallelHostedServices : IHostedService
{
    private readonly IReadOnlyCollection<HostedServiceWrapper> _hostedServices;

    public ParallelHostedServices(params HostedServiceWrapper[] hostedServices)
    {
        _hostedServices = hostedServices ?? throw new ArgumentNullException(nameof(hostedServices));
    }

    internal void SetParent(HostedServiceWrapper parent)
    {
        foreach (var hostedService in _hostedServices)
        {
            hostedService.Parent = parent;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken) =>
        await Task
            .WhenAll(_hostedServices
                .Select(hostedService => hostedService.StartAsync(cancellationToken)));

    public Task StopAsync(CancellationToken cancellationToken)
    {
        var tasks = _hostedServices
            .Select(hostedService => hostedService.StopAsync(cancellationToken));
        return Task.WhenAll(tasks);
    }
}