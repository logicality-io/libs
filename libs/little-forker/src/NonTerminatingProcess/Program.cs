using System.Diagnostics;
using Logicality.LittleForker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;



var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var configRoot = new ConfigurationBuilder()
    .AddCommandLine(args)
    .AddEnvironmentVariables()
    .Build();

// Running program with --debug=true will attach a debugger.
// Used to assist with debugging LittleForker.
if (configRoot.GetValue("debug", false))
{
    Debugger.Launch();
}

var ignoreShutdownSignal = configRoot.GetValue("ignore-shutdown-signal", false);
if (ignoreShutdownSignal)
{
    logger.Information("Will ignore Shutdown Signal");
}

var exitWithNonZero = configRoot.GetValue("exit-with-non-zero", false);
if (exitWithNonZero)
{
    logger.Information("Will exit with non-zero exit code");
}

var shutdown  = new CancellationTokenSource(TimeSpan.FromSeconds(100));
var pid       = Environment.ProcessId;
var parentPid = configRoot.GetValue<int?>("ParentProcessId");

logger.Information($"Long running process started. PID={pid}");

using (parentPid.HasValue
    ? new ProcessExitedHelper(parentPid.Value, _ => ParentExited(parentPid.Value), new NullLoggerFactory())
    : NoopDisposable.Instance)
{
    using (await CooperativeShutdown.Listen(ExitRequested, new NullLoggerFactory()))
    {
        // Poll the shutdown token in a tight loop
        while (!shutdown.IsCancellationRequested || ignoreShutdownSignal)
        {
            await Task.Delay(100);
        }
        logger.Information("Exiting.");
    }
}

return exitWithNonZero ? -1 : 0;

void ExitRequested()
{
    logger.Information("Cooperative shutdown requested.");

    if (ignoreShutdownSignal)
    {
        Log.Logger.Information("Shut down signal ignored.");
        return;
    }
    shutdown.Cancel();
}

void ParentExited(int processId)
{
    logger.Information($"Parent process {processId} exited.");
    shutdown.Cancel();
}

class NoopDisposable : IDisposable
{
    public void Dispose()
    { }

    internal static readonly IDisposable Instance = new NoopDisposable();
}

/*
internal sealed class Program
{
    // Yeah this process is supposed to be "non-terminating"
    // but we don't want tons of orphaned instances running
    // because of tests so it terminates after a long
    // enough time (100 seconds)
    private readonly CancellationTokenSource _shutdown = new(TimeSpan.FromSeconds(100));
    private readonly IConfigurationRoot      _configRoot;
    private readonly bool                    _ignoreShutdownSignal;
    private readonly bool                    _exitWithNonZero;

    static Program()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

    private Program(string[] args)
    {
        _configRoot = new ConfigurationBuilder()
            .AddCommandLine(args)
            .AddEnvironmentVariables()
            .Build();

        // Running program with --debug=true will attach a debugger.
        // Used to assist with debugging LittleForker.
        if (_configRoot.GetValue("debug", false)) 
        {
            Debugger.Launch();
        }
            
        _ignoreShutdownSignal = _configRoot.GetValue<bool>("ignore-shutdown-signal", false);
        if (_ignoreShutdownSignal)
        {
            Log.Logger.Information("Will ignore Shutdown Signal");
        }

        _exitWithNonZero = _configRoot.GetValue<bool>("exit-with-non-zero", false);
        if (_exitWithNonZero)
        {
            Log.Logger.Information("Will exit with non-zero exit code");
        }
    }

    private async Task<int> Run()
    {
        var pid = Process.GetCurrentProcess().Id;
        Log.Logger.Information($"Long running process started. PID={pid}");

        var parentPid = _configRoot.GetValue<int?>("ParentProcessId");

        using (parentPid.HasValue
            ? new ProcessExitedHelper(parentPid.Value, _ => ParentExited(parentPid.Value), new NullLoggerFactory())
            : NoopDisposable.Instance)
        {
            using (await CooperativeShutdown.Listen(ExitRequested, new NullLoggerFactory()))
            {
                // Poll the shutdown token in a tight loop
                while(!_shutdown.IsCancellationRequested || _ignoreShutdownSignal)
                {
                    await Task.Delay(100);
                }
                Log.Information("Exiting.");
            }
        }

        return _exitWithNonZero ? -1 : 0;
    }

    static Task<int> Main(string[] args) => new Program(args).Run();

    private void ExitRequested()
    {
        Log.Logger.Information("Cooperative shutdown requested.");

        if (_ignoreShutdownSignal)
        {
            Log.Logger.Information("Shut down signal ignored.");
            return;
        }

        _shutdown.Cancel();
    }

    private void ParentExited(int processId)
    {
        Log.Logger.Information($"Parent process {processId} exited.");
        _shutdown.Cancel();
    }

    private class NoopDisposable : IDisposable
    {
        public void Dispose()
        {}

        internal static readonly IDisposable Instance = new NoopDisposable();
    }
}*/