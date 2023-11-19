namespace Logicality.Lambda.TestHost;

/// <summary>
/// Activates a LambdaFunction instance using default paramaterless constructor.
/// </summary>
public class DefaultLambdaFunctionActivator(Type type) : ILambdaFunctionActivator
{
    public object Activate() => Activator.CreateInstance(type)!;
}