using Amazon.Lambda.TestUtilities;
using Logicality.Lambda.Example;
using Xunit;

namespace Logicality.Lambda
{
    public class AsynchronousInvokeFunctionTests
    {
        [Fact]
        public async Task Can_activate_lambda()
        {
            var testFunction = new ExampleAsynchronousInvokeFunction();

            await testFunction.Handle(new Request{ Url = "http://example.com"}, new TestLambdaContext());
        }
    }
}