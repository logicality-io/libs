using System.Threading.Tasks;
using Amazon.Lambda.TestUtilities;
using Logicality.Lambda.Example;
using Shouldly;
using Xunit;

namespace Logicality.Lambda
{
    public class SynchronousFunctionBaseTests
    {
        [Fact]
        public async Task Can_activate_lambda()
        {
            var testFunction = new ExampleSynchronousFunction(_ => {});

            var result = await testFunction.Handle("foo", new TestLambdaContext());

            result.ShouldNotBeNullOrEmpty();
        }
    }
}
