namespace Logicality.Lambda.TestHost;

/// <summary>
/// Activates a LambdaFunction instance using default paramaterless constructor.
/// </summary>
public class DefaultLambdaFunctionActivator : ILambdaFunctionActivator
{
    private readonly Type _type;

    public DefaultLambdaFunctionActivator(Type type)
    {
        _type = type;
    }

    public object Activate() => Activator.CreateInstance(_type)!;
}