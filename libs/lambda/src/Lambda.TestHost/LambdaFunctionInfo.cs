using System.Reflection;
using Amazon.Lambda.Core;

namespace Logicality.Lambda.TestHost;

public class LambdaFunctionInfo : ILambdaFunctionInfo
{
    /// <summary>
    ///     Information about a lambda function that can be invoked.
    /// </summary>
    /// <param name="name">
    ///     The name of the function.
    /// </param>
    /// <param name="functionType">
    ///     The lambda function type that will be activated.
    /// </param>
    /// <param name="handlerMethod">
    ///     The lambda function method that will be invoked.
    /// </param>
    /// <param name="reservedConcurrency">
    ///     The reserved concurrency.
    /// </param>
    /// <param name="functionActivator">
    ///     The object activator. If null the <see cref="DefaultLambdaFunctionActivator"/> will be used.
    ///     If you want customise your function activation, e.g. inject some config/settings into non
    /// </param>
    public LambdaFunctionInfo(
        string        name,
        Type          functionType,
        string        handlerMethod,
        int?          reservedConcurrency = null,
        Func<object>? functionActivator   = null)
    {
        Name                = name;
        Type                = functionType;
        HandlerMethod       = functionType.GetMethod(handlerMethod, BindingFlags.Public | BindingFlags.Instance)!;
        ReservedConcurrency = reservedConcurrency ?? int.MaxValue;
        FunctionActivator = functionActivator == null
            ? new DefaultLambdaFunctionActivator(functionType)
            : new DelegateLambdaFunctionActivator(functionActivator);


        // Search to see if a Lambda serializer is registered.
        var attribute = HandlerMethod.GetCustomAttribute(typeof(LambdaSerializerAttribute)) as LambdaSerializerAttribute ??
                        functionType.Assembly.GetCustomAttribute(typeof(LambdaSerializerAttribute)) as LambdaSerializerAttribute;

        if (attribute != null)
        {
            Serializer = (Activator.CreateInstance(attribute.SerializerType) as ILambdaSerializer)!;
        }


        HandlerString = $"{functionType.Assembly.GetName().Name}::{functionType.FullName}::{handlerMethod}";
    }

    public Type Type { get; }

    public string Name { get; }

    public MethodInfo HandlerMethod { get; }

    public ILambdaSerializer? Serializer { get; } = null;

    public int ReservedConcurrency { get; }

    public string HandlerString { get; }

    public ILambdaFunctionActivator FunctionActivator { get; }
}