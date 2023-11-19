using Microsoft.Extensions.Hosting;

namespace Logicality.Extensions.Hosting;

public class ExampleHostedService(Context context) : IHostedService
{
    public bool OnStartCalled { get; set; }

    public bool OnStopCalled { get; set; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        OnStartCalled = true;
        context.Increment();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        OnStopCalled = true;
        return Task.CompletedTask;
    }
}