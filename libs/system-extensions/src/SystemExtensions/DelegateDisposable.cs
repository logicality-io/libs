using System;
using System.Threading.Tasks;

namespace Logicality.SystemExtensions;

public class DelegateDisposable : IDisposable
{
    private readonly Action _onDispose;

    public DelegateDisposable(Action onDispose) 
        => _onDispose = onDispose;

    public void Dispose() => _onDispose();
}