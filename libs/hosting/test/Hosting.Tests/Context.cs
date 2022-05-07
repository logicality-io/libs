using System.Threading;

namespace Logicality.Extensions.Hosting;

public class Context
{
    private int _counter;

    public int Counter => _counter;

    public void Increment() => Interlocked.Increment(ref _counter);
}