using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Logicality.Lambda;

/// <summary>
/// A base function the is invoked synchronously (see https://docs.aws.amazon.com/lambda/latest/dg/invocation-sync.html)
/// that encapsulates some boiler plate code around configuration, logging and dependency injection.
/// 
/// By default configuration is loaded from environment variables, appsettings.json and appsettings.{environment}.json
/// </summary>
/// <typeparam name="TOptions">The configuration object that configuration will be bound to.</typeparam>
/// <typeparam name="THandler">The handler that will be activated to handle the request.</typeparam>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public abstract class SynchronousInvokeFunction<TRequest, TResponse, TOptions, THandler> : FunctionBase<TOptions, THandler>
    where TOptions : class, new()
    where THandler: class, ISynchronousInvokeHandler<TRequest, TResponse>
{
    public Task<TResponse> Handle(TRequest input, ILambdaContext context)
    {
        var serviceProvider = GetServiceProvider();
        var handler         = serviceProvider.GetRequiredService<THandler>();
        return handler.Handle(input, context);
    }
}