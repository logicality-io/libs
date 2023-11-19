using Amazon.Lambda.Core;
using Microsoft.Extensions.Options;

namespace Logicality.Lambda.Example;

public class ExampleAsynchronousInvokeHandler(
    IHttpClientFactory               clientFactory,
    IOptionsSnapshot<ExampleOptions> optionsSnapshot)
    : AsynchronousInvokeHandler<Request, ExampleOptions>(optionsSnapshot)
{
    public override async Task Handle(Request request, ILambdaContext context)
    {
        var httpClient = clientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(Options.Timeout);
        await httpClient.GetAsync(request.Url);
    }
}