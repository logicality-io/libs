using System;

namespace Logicality.SystemExtensions
{
    /// <summary>
    /// A disposable that invokes the supplied Action.
    /// </summary>
    public class DisposableAction : IDisposable
    {
        private readonly Action _onDispose;

        public DisposableAction(Action onDispose) 
            => _onDispose = onDispose;

        public void Dispose() => _onDispose();
    }
}
