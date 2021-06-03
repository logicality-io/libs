using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Logicality.Lambda
{
    /// <summary>
    /// Represents a synchronus lambda handler.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface ISynchronousHandler<TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest input, ILambdaContext context);
    }
}