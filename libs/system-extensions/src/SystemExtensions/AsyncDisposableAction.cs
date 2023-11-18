namespace Logicality.SystemExtensions;

/// <summary>
/// An async disposable that invokes the supplied Action.
/// </summary>
public class AsyncDisposableAction : IAsyncDisposable
{
    private readonly Func<ValueTask> _onDispose;

    public AsyncDisposableAction(Func<ValueTask> onDispose)
    {
        _onDispose = onDispose;
    }

    public ValueTask DisposeAsync() => _onDispose();
}