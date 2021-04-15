using System.Net.Http;
using Amazon.Lambda.Core;

namespace Logicality.Lambda.Example
{
    public class Handler
    {
        private readonly IHttpClientFactory _clientFactory;

        public Handler(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public string? Handle(string input, ILambdaContext context)
        {
            return input?.ToUpper();
        }
    }
}