namespace Logicality.SystemExtensions;

/// <summary>
/// An async disposable that invokes the supplied Action.
/// </summary>
public class AsyncDisposableAction(Func<ValueTask> onDispose) : IAsyncDisposable
{
    public ValueTask DisposeAsync() => onDispose();
}