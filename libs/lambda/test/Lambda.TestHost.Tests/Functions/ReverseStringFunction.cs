using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace Logicality.AWS.Lambda.TestHost.Functions
{
    public class ReverseStringFunction
    {
        [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
        public string Reverse(string input, ILambdaContext context)
        {
            return new string(input.Reverse().ToArray());
        }
    }
}