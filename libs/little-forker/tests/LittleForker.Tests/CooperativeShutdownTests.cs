using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Logicality.LittleForker;

public class CooperativeShutdownTests
{
    private readonly ILoggerFactory _loggerFactory;

    public CooperativeShutdownTests(ITestOutputHelper outputHelper)
    {
        _loggerFactory = new XunitLoggerFactory(outputHelper).LoggerFactory;
    }

    [Fact]
    public async Task When_server_signals_exit_then_should_notify_client_to_exit()
    {
        var exitCalled = new TaskCompletionSource<bool>();
        var listener = await CooperativeShutdown.Listen(
            () => exitCalled.SetResult(true),
            _loggerFactory);

        await CooperativeShutdown.SignalExit(Environment.ProcessId, _loggerFactory);

        (await exitCalled.Task).ShouldBeTrue();

        listener.Dispose();
    }
}