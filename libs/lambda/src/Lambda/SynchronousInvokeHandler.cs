using Amazon.Lambda.Core;
using Microsoft.Extensions.Options;

namespace Logicality.Lambda
{
    /// <summary>
    /// A implementation of <see cref="IAsynchronousInvokeHandler{TInput}"/> that has
    /// an <see cref="IOptionsSnapshot{TOptions}"/> injected.
    /// </summary>
    public abstract class SynchronousInvokeHandler<TInput, TResponse, TOptions>(
        IOptionsSnapshot<TOptions> optionsSnapshot) : ISynchronousInvokeHandler<TInput, TResponse>
        where TOptions : class, new()
    {
        protected TOptions Options { get; } = optionsSnapshot.Value;

        public abstract Task<TResponse> Handle(TInput input, ILambdaContext context);
    }
}