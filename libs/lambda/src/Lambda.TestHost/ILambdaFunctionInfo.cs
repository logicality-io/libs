using System.Reflection;
using Amazon.Lambda.Core;

namespace Logicality.Lambda.TestHost;

public interface ILambdaFunctionInfo
{
    MethodInfo HandlerMethod { get; }

    int ReservedConcurrency { get; }

    ILambdaFunctionActivator FunctionActivator { get; }

    ILambdaSerializer? Serializer { get; }

    string Name { get; }
}