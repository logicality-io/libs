using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Logicality.Lambda.Example;

public class SynchronousHandler: ISynchronousHandler<string, string>
{
    private readonly IHttpClientFactory _clientFactory;

    public SynchronousHandler(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public Task<string> Handle(string input, ILambdaContext context) 
        => Task.FromResult(input?.ToUpper())!;
}