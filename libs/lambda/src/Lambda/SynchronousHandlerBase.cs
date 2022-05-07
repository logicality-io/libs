using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Options;

namespace Logicality.Lambda;

/// <summary>
/// A implementation of <see cref="IAsynchronousHandler{TRequest}"/> that has
/// an <see cref="IOptionsSnapshot{TOptions}"/> injected.
/// </summary>
public abstract class SynchronousHandlerBase<TRequest, TResponse, TOptions>
    : ISynchronousHandler<TRequest, TResponse> where TOptions : class, new()
{
    protected SynchronousHandlerBase(IOptionsSnapshot<TOptions> optionsSnapshot)
    {
        OptionsValue = optionsSnapshot.Value;
    }

    protected TOptions OptionsValue { get; }

    public abstract Task<TResponse> Handle(TRequest input, ILambdaContext context);
}