using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Logicality.Lambda
{
    /// <summary>
    /// A base function that is invoked asynchronously (see https://docs.aws.amazon.com/lambda/latest/dg/invocation-async.html),
    /// that encapsulates some boiler plate code around configuration, logging and dependency injection. 
    /// 
    /// By default configuration is loaded from environment variables, appsettings.json and appsettings.{environment}.json
    /// </summary>
    /// <typeparam name="THandler">The handler that will be activated to handle the request.</typeparam>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TOptions">The options type.</typeparam>
    public abstract class AsynchronousInvokeFunction<TRequest, TOptions, THandler> : FunctionBase<TOptions, THandler>
        where THandler : class, IAsynchronousInvokeHandler<TRequest> where TOptions : class, new()
    {
        public Task Handle(TRequest input, ILambdaContext context)
        {
            var serviceProvider = GetServiceProvider();
            var handler = serviceProvider.GetRequiredService<THandler>();
            return handler.Handle(input, context);
        }
    }
}