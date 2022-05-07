using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Logicality.Lambda;

/// <summary>
/// Represents an asynchronous lambda handler
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IAsynchronousHandler<TRequest>
{
    Task Handle(TRequest input, ILambdaContext context);
}