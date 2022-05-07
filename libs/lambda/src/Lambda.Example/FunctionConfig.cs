using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace Logicality.Lambda.Example;

public class FunctionOptions
{
    public string? Foo { get; set; }
}