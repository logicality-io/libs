using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Options;

namespace Logicality.Lambda
{
    /// <summary>
    /// A implementation of <see cref="IAsynchronousHandler{TRequest}"/> that has
    /// an <see cref="IOptionsSnapshot{TOptions}"/> injected.
    /// </summary>
    /// <typeparam name="TRequest">The type the handler will handle.</typeparam>
    /// <typeparam name="TOptions">The type of the options that will be injected via constructor.</typeparam>
    public abstract class AsynchronousHandlerBase<TRequest, TOptions> :
        IAsynchronousHandler<TRequest>
        where TOptions : class, new()
    {
        protected AsynchronousHandlerBase(IOptionsSnapshot<TOptions> optionsSnapshot)
        {
            OptionsValue = optionsSnapshot.Value;
        }

        /// <summary> 
        /// The options vlaue. 
        /// </summary>
        protected TOptions OptionsValue { get; }

        /// <summary>
        /// The handler.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task Handle(TRequest input, ILambdaContext context);
    }
}