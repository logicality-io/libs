namespace Logicality.Lambda.TestHost;

internal class LambdaInstance(ILambdaFunctionInfo lambdaFunction)
{
    public Guid InstanceId { get; } = Guid.NewGuid();

    public ILambdaFunctionInfo LambdaFunction { get; } = lambdaFunction;

    public object FunctionInstance { get; } = lambdaFunction.FunctionActivator.Activate();
}