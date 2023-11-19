using Amazon.Lambda.Core;
using Microsoft.Extensions.Options;

namespace Logicality.Lambda.Example;

public class ExampleSynchronousInvokeHandler(
    IHttpClientFactory               clientFactory,
    IOptionsSnapshot<ExampleOptions> optionsSnapshot)
    : SynchronousInvokeHandler<Request, Response, ExampleOptions>(optionsSnapshot)
{
    public override async Task<Response> Handle(Request request, ILambdaContext context)
    {
        var httpClient = clientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(Options.Timeout);
        var response = await httpClient.GetAsync(request.Url);
        var body     = await response.Content.ReadAsStringAsync();
        return new Response(body);
    }
}