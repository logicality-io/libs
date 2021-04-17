using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;

namespace Logicality.AWS.Lambda.TestHost.LocalStack
{
    public class SQSLambdaFunction
    {
        [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
        public Task FunctionHandler(SQSEvent request, ILambdaContext lambdaContext)
        {
            return Task.CompletedTask;
        }
    }
}