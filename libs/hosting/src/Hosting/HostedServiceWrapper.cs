using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Logicality.Extensions.Hosting;

public class HostedServiceWrapper : IHostedService
{
    private readonly IHostedService          _inner;
    private readonly ILogger<IHostedService> _logger;

    internal HostedServiceWrapper(IHostedService inner, ILogger<HostedServiceWrapper> logger)
    {
        _inner  = inner;
        _logger = logger;
        Name    = inner.GetType().Name.Replace("HostedService", "");
    }

    public string Name { get; internal set; }

    public HostedServiceWrapper? Parent { get; internal set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scope = _logger.BeginScope(GetScopeState());

        _logger.LogInformation("Starting...");
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _inner.StartAsync(cancellationToken);
            var elapsed = Math.Round(stopwatch.Elapsed.TotalMilliseconds, 0).ToString(CultureInfo.InvariantCulture);
            _logger.LogInformation($"Took {elapsed}ms to start.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw;
        }
        finally
        {
            scope.Dispose();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var scope = _logger.BeginScope(GetScopeState());

        _logger.LogInformation("Stopping...");
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _inner.StopAsync(cancellationToken);
            _logger.LogInformation($"took {stopwatch.Elapsed.TotalMilliseconds}ms to stop");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw;
        }
        finally
        {
            scope.Dispose();
        }
    }

    private KeyValuePair<string, object>[] GetScopeState()
        => new[] { new KeyValuePair<string, object>("HostedService", GetScopeValue()) };

    private string GetScopeValue()
    {
        var scope  = Name;
        var parent = Parent;
        while (parent != null)
        {
            scope  = $"{parent.Name}/{scope}";
            parent = parent.Parent;
        }
        return scope;
    }
}