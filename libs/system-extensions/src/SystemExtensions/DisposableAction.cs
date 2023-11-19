namespace Logicality.SystemExtensions;

/// <summary>
/// A disposable that invokes the supplied Action.
/// </summary>
public class DisposableAction(Action onDispose) : IDisposable
{
    public void Dispose() => onDispose();
}