namespace Logicality.GitHubActionsWorkflowBuilder
{
    // TODO replace with system extensions package reference.
    internal class DisposableAction : IDisposable
    {
        private readonly Action _onDispose;

        public DisposableAction(Action onDispose) 
            => _onDispose = onDispose;

        public void Dispose() => _onDispose();
    }
}
