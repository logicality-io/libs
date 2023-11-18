using Amazon.Lambda.Core;
using Microsoft.Extensions.Options;

namespace Logicality.Lambda.Example;

public class ExampleAsynchronousInvokeHandler : AsynchronousInvokeHandler<Request, ExampleOptions>
{
    private readonly IHttpClientFactory _clientFactory;

    public ExampleAsynchronousInvokeHandler(IHttpClientFactory clientFactory, IOptionsSnapshot<ExampleOptions> optionsSnapshot) 
        : base(optionsSnapshot)
    {
        _clientFactory = clientFactory;
    }

    public override async Task Handle(Request request, ILambdaContext context)
    {
        var httpClient = _clientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(Options.Timeout);
        await httpClient.GetAsync(request.Url);
    }
}