using System.Collections.Concurrent;

namespace Logicality.Lambda.TestHost;

internal class LambdaInstancePool(ILambdaFunctionInfo lambdaFunctionInfo)
{
    private readonly ConcurrentQueue<LambdaInstance>  _availableInstances = new();
    private readonly Dictionary<Guid, LambdaInstance> _usedInstances      = new();
    private          int                              _counter;

    public LambdaInstance? Get()
    {
        lock (_availableInstances)
        {
            if (_availableInstances.TryDequeue(out var result))
            {
                _usedInstances.Add(result.InstanceId, result);
                return result;
            }

            if (_counter >= lambdaFunctionInfo.ReservedConcurrency)
            {
                return null;
            }

            var instance = new LambdaInstance(lambdaFunctionInfo);
            _counter++;
            _usedInstances.Add(instance.InstanceId, result!);
            return instance;
        }
    }

    public void Return(LambdaInstance lambdaInstance)
    {
        lock (_availableInstances)
        {
            _usedInstances.Remove(lambdaInstance.InstanceId);
            _availableInstances.Enqueue(lambdaInstance);
        }
    }
}