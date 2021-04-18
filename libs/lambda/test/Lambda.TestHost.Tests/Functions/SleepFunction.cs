using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace Logicality.AWS.Lambda.TestHost.Functions
{
    public class SleepFunction
    {
        [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
        public async Task Handle(int sleep, ILambdaContext context)
        {
            await Task.Delay(sleep);
        }
    }
}