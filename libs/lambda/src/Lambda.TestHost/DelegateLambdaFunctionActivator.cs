namespace Logicality.Lambda.TestHost;

internal class DelegateLambdaFunctionActivator(Func<object> activate) : ILambdaFunctionActivator
{
    public object Activate() => activate();
}