using System;

namespace Logicality.Lambda.TestHost;

internal class LambdaInstance
{
    public LambdaInstance(LambdaFunctionInfo lambdaFunction)
    {
        LambdaFunction   = lambdaFunction;
        FunctionInstance = Activator.CreateInstance(lambdaFunction.Type)!;
        InstanceId       = Guid.NewGuid();
    }

    public Guid InstanceId { get; }

    public LambdaFunctionInfo LambdaFunction { get; }

    public object FunctionInstance { get; }
}