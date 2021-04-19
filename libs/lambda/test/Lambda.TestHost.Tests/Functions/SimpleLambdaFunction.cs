using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace Logicality.Lambda.TestHost.Functions
{
    public class SimpleLambdaFunction
    {
        [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
        public SimpleResponse FunctionHandler(SimpleRequest request, ILambdaContext lambdaContext)
        {
            return new SimpleResponse
            {
                Foo = request.Foo
            };
        }

        public class SimpleRequest
        {
            public string Foo { get; set; }
        }

        public class SimpleResponse
        {
            public string Foo { get; set; }
        }
    }
}
