namespace Logicality.SystemExtensions;

public class AsyncDelegateDisposable : IAsyncDisposable
{
    private readonly Func<ValueTask> _onDispose;

    public AsyncDelegateDisposable(Func<ValueTask> onDispose)
        => _onDispose = onDispose;

    public ValueTask DisposeAsync() => _onDispose();
}