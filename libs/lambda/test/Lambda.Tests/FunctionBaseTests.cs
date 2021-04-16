using Amazon.Lambda.TestUtilities;
using Logicality.Lambda.Example;
using Xunit;

namespace Logicality.Extensions.Configuration
{
    public class FunctionBaseTests
    {
        [Fact]
        public void Can_activate_lambda()
        {
            var testFunction = new ExampleFunction(configuration => {});

            testFunction.Handle("foo", new TestLambdaContext());
        }
    }
}
