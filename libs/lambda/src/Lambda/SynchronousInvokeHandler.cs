using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Options;

namespace Logicality.Lambda
{
    /// <summary>
    /// A implementation of <see cref="IAsynchronousInvokeHandler{TInput}"/> that has
    /// an <see cref="IOptionsSnapshot{TOptions}"/> injected.
    /// </summary>
    public abstract class SynchronousInvokeHandler<TInput, TResponse, TOptions>
        : ISynchronousInvokeHandler<TInput, TResponse> where TOptions : class, new()
    {
        protected SynchronousInvokeHandler(IOptionsSnapshot<TOptions> optionsSnapshot)
        {
            Options = optionsSnapshot.Value;
        }

        protected TOptions Options { get; }

        public abstract Task<TResponse> Handle(TInput input, ILambdaContext context);
    }
}