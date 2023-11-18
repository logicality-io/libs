using Amazon.Lambda.Core;

namespace Logicality.Lambda;

/// <summary>
/// Represents an lambda handler for that is synchronously invoked (returns a response object).
/// </summary>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface ISynchronousInvokeHandler<TInput, TResponse>
{
    Task<TResponse> Handle(TInput input, ILambdaContext context);
}