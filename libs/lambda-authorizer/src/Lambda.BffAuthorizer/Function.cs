using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using Logicality.Lambda;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace Lambda.BffAuthorizer
{
    public class BffAuthorizerFunction : FunctionBase<BffAuthorizerConfig, BffAuthorizerHandler>
    {
        public Task<APIGatewayCustomAuthorizerResponse> Handle(APIGatewayCustomAuthorizerRequest request, ILambdaContext context)
        {
            var handler = ServiceProvider.GetRequiredService<BffAuthorizerHandler>();
            return handler.Handle(request, context);
        }
    }

    public class BffAuthorizerHandler
    {
        public async Task<APIGatewayCustomAuthorizerResponse> Handle(APIGatewayCustomAuthorizerRequest request, ILambdaContext context)
        {
        }
    }

    public class BffAuthorizerConfig{}
}
