using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Options;

namespace Logicality.Lambda;

/// <summary>
/// A implementation of <see cref="IAsynchronousInvokeHandler{TRequest}"/> that has
/// an <see cref="IOptionsSnapshot{TOptions}"/> injected.
/// </summary>
/// <typeparam name="TRequest">The type the handler will handle.</typeparam>
/// <typeparam name="TOptions">The type of the options that will be injected via constructor.</typeparam>
public abstract class AsynchronousInvokeHandler<TRequest, TOptions> :
    IAsynchronousInvokeHandler<TRequest>
    where TOptions : class, new()
{
    protected AsynchronousInvokeHandler(IOptionsSnapshot<TOptions> optionsSnapshot) 
        => Options = optionsSnapshot.Value;

    /// <summary> 
    /// The options value. 
    /// </summary>
    protected TOptions Options { get; }

    /// <summary>
    /// The handler.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public abstract Task Handle(TRequest input, ILambdaContext context);
}