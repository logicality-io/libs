using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace Logicality.Lambda.TestHost;

public class SimpleLambdaFunction
{
    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public SimpleResponse FunctionHandler(SimpleRequest request, ILambdaContext _)
    {
        var reverseData = new string(request.Data.Reverse().ToArray());
        return new SimpleResponse(reverseData);
    }

    public record SimpleRequest(string Data);

    public record SimpleResponse(string Reverse);
}