using System.Threading.Tasks;
using Amazon.Lambda.TestUtilities;
using Logicality.Lambda.Example;
using Xunit;

namespace Logicality.Lambda
{
    public class AsynchronousFunctionBaseTests
    {
        [Fact]
        public async Task Can_activate_lambda()
        {
            var testFunction = new ExampleAsynchronousInvokeFunction();

            await testFunction.Handle("foo", new TestLambdaContext());
        }
    }
}