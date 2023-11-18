namespace Logicality.Lambda.TestHost;

internal class DelegateLambdaFunctionActivator : ILambdaFunctionActivator
{
    private readonly Func<object> _activate;

    public DelegateLambdaFunctionActivator(Func<object> activate)
    {
        _activate = activate;
    }

    public object Activate() => _activate();
}