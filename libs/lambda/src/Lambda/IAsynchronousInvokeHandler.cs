using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Logicality.Lambda;

/// <summary>
/// Represents an lambda handler for that is asynchronously invoked (returns void).
/// </summary>
/// <typeparam name="TRequest">A the request inout type being handled</typeparam>
public interface IAsynchronousInvokeHandler<TRequest>
{
    /// <summary>
    /// Handles the function request
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    Task Handle(TRequest input, ILambdaContext context);
}