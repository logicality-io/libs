using Microsoft.Extensions.Hosting;

namespace Logicality.Extensions.Hosting;

/// <summary>
/// An <see cref="IHostedService"/> that contains child hosted services, starting them sequential
/// in the order that they are supplied. This is important for services that may need to
/// explicitly start before services.
/// </summary>
public class SequentialHostedServices : IHostedService
{
    private readonly IReadOnlyCollection<HostedServiceWrapper> _hostedServices;

    public SequentialHostedServices(params HostedServiceWrapper[] hostedServices)
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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var hostedService in _hostedServices)
        {
            await hostedService.StartAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        var tasks = _hostedServices
            .Select(hostedService => hostedService.StopAsync(cancellationToken));
        return Task.WhenAll(tasks);
    }
}