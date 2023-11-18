using Amazon.Lambda.Core;

namespace Logicality.Lambda;

/// <summary>
/// Represents an lambda handler for that is asynchronously invoked (returns void).
/// </summary>
/// <typeparam name="TInput">The request input type being handled.</typeparam>
public interface IAsynchronousInvokeHandler<TInput>
{
    /// <summary>
    /// Handles the function request
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    Task Handle(TInput input, ILambdaContext context);
}