using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Logicality.Lambda.Example
{
    public class AsynchronousHandler : IAsynchronousHandler<string>
    {
        private readonly IHttpClientFactory _clientFactory;

        public AsynchronousHandler(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public Task Handle(string input, ILambdaContext context)
            => Task.CompletedTask;
    }
}