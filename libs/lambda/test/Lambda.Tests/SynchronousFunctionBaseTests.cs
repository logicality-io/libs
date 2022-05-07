using System.Threading.Tasks;
using Amazon.Lambda.TestUtilities;
using Logicality.Lambda.Example;
using Shouldly;
using Xunit;

namespace Logicality.Lambda;

public class SynchronousFunctionBaseTests
{
    [Fact]
    public async Task Can_activate_lambda()
    {
        var testFunction = new ExampleSynchronousInvokeFunction();

        var request = new Request
        {
            Url = "http://example.com"
        };
        var response = await testFunction.Handle(request, new TestLambdaContext());

        response.Body.ShouldNotBeNullOrEmpty();
    }
}