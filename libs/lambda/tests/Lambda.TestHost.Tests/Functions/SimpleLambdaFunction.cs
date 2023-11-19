using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace Logicality.Lambda.TestHost.Functions;

public class SimpleLambdaFunction
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public SimpleResponse FunctionHandler(SimpleRequest request, ILambdaContext _) => new(request.Foo);

    public record SimpleRequest(string Foo);

    public record SimpleResponse(string Foo);
}