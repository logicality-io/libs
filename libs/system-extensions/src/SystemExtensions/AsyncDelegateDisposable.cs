namespace Logicality.SystemExtensions;

public class AsyncDelegateDisposable(Func<ValueTask> onDispose) : IAsyncDisposable
{
    public ValueTask DisposeAsync() => onDispose();
}