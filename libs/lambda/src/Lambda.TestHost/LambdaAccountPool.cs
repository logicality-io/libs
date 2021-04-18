using System;
using System.Collections.Generic;
using System.Threading;

namespace Logicality.AWS.Lambda.TestHost
{
    internal class LambdaAccountPool
    {
        private readonly uint _accountConcurrencyLimit;
        private int _counter;

        private readonly Dictionary<string, LambdaInstancePool> _instancePools
            = new Dictionary<string, LambdaInstancePool>(StringComparer.OrdinalIgnoreCase);

        public LambdaAccountPool(
            uint accountConcurrencyLimit,
            IReadOnlyDictionary<string, LambdaFunctionInfo> lambdaFunctionInfos)
        {
            _accountConcurrencyLimit = accountConcurrencyLimit;

            foreach (var lambdaFunctionInfo in lambdaFunctionInfos)
            {
                _instancePools.Add(lambdaFunctionInfo.Key, new LambdaInstancePool(lambdaFunctionInfo.Value));
            }
        }

        public LambdaInstance? Get(string functionName)
        {
            var c = Interlocked.Increment(ref _counter);
            if (c > _accountConcurrencyLimit)
            {
                Interlocked.Decrement(ref _counter);
                return null;
            }

            return _instancePools.TryGetValue(functionName, out var item)
                ? item.Get()
                : null;
        }

        public void Return(LambdaInstance lambdaInstance)
        {
            Interlocked.Decrement(ref _counter);
            _instancePools[lambdaInstance.LambdaFunction.Name].Return(lambdaInstance);
        }
    }
}