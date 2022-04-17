namespace Logicality.GitHub.Actions.Workflow;

internal class DelegateDisposable : IDisposable
{
    private readonly Action _onDispose;

    public DelegateDisposable(Action onDispose) 
        => _onDispose = onDispose;

    public void Dispose() => _onDispose();
}
