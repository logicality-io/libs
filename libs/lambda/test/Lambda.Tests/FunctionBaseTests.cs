using Amazon.Lambda.TestUtilities;
using Logicality.Lambda.Example;
using Shouldly;
using Xunit;

namespace Logicality.Lambda
{
    public class FunctionBaseTests
    {
        [Fact]
        public void Can_activate_lambda()
        {
            var testFunction = new ExampleFunction(_ => {});

            var result = testFunction.Handle("foo", new TestLambdaContext());

            result.ShouldNotBeNullOrEmpty();
        }
    }
}
