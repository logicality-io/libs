using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace Logicality.AWS.Lambda.TestHost.Functions
{
    public class BrokenFunction
    {
        [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
        public void Handle(int sleep, ILambdaContext context)
        {
            throw new Exception();
        }
    }
}