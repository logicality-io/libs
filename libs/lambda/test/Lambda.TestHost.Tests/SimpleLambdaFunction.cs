using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace Logicality.AWS.Lambda.TestHost
{
    public class SimpleLambdaFunction
    {
        [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
        public SimpleResponse FunctionHandler(SimpleRequest request, ILambdaContext lambdaContext)
        {
            return new SimpleResponse
            {
                Reverse = new string(request.Data.Reverse().ToArray())
            };
        }

        public class SimpleRequest
        {
            public string Data { get; set; }
        }

        public class SimpleResponse
        {
            public string Reverse { get; set; }
        }
    }
}