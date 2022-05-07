using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Options;

namespace Logicality.Lambda.Example;

public class ExampleSynchronousInvokeHandler: SynchronousInvokeHandler<Request, Response, ExampleOptions>
{
    private readonly IHttpClientFactory _clientFactory;

    public ExampleSynchronousInvokeHandler(
        IHttpClientFactory               clientFactory,
        IOptionsSnapshot<ExampleOptions> optionsSnapshot) 
        : base(optionsSnapshot)
    {
        _clientFactory = clientFactory;
    }

    public override async Task<Response> Handle(Request request, ILambdaContext context)
    {
        var httpClient = _clientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(Options.Timeout);
        var response = await httpClient.GetAsync(request.Url);
        var body     = await response.Content.ReadAsStringAsync();
        return new Response
        {
            Body = body
        };
    }
}