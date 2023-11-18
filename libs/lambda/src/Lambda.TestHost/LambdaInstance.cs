namespace Logicality.Lambda.TestHost;

internal class LambdaInstance
{
    public LambdaInstance(ILambdaFunctionInfo lambdaFunction)
    {
        LambdaFunction   = lambdaFunction;
        FunctionInstance = lambdaFunction.FunctionActivator.Activate();
        InstanceId       = Guid.NewGuid();
    }

    public Guid InstanceId { get; }

    public ILambdaFunctionInfo LambdaFunction { get; }

    public object FunctionInstance { get; }
}