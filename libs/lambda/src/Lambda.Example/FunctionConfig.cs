using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace Logicality.Lambda.Example
{
    public class FunctionConfig
    {
        public string? Foo { get; set; }
    }
}
