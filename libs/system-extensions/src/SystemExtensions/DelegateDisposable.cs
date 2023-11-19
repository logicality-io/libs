namespace Logicality.SystemExtensions;

public class DelegateDisposable(Action onDispose) : IDisposable
{
    public void Dispose() => onDispose();
}