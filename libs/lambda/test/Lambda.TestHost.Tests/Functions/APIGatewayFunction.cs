using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace Logicality.AWS.Lambda.TestHost.Functions
{
    public class APIGatewayFunction
    {
        [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
        public APIGatewayProxyResponse Handle(APIGatewayProxyRequest apiGatewayProxyRequest, ILambdaContext context)
        {
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = $"Hello AWS Serverless{apiGatewayProxyRequest.Path}",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain" }
                }
            };

            return response;
        }
    }
}