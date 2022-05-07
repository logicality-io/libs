using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Logicality.Lambda;

/// <summary>
/// Represents an lambda handler for that is synchronously invoked (returns a response object).
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface ISynchronousInvokeHandler<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest input, ILambdaContext context);
}