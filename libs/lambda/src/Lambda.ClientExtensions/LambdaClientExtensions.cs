using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Model;

// ReSharper disable once CheckNamespace
namespace Amazon.Lambda
{
    public static class LambdaClientExtensions
    {
        /// <summary>
        /// Serializes a payloadObject object and invokes it against the supplied function name.
        /// </summary>
        /// <typeparam name="T">The payloadObject object </typeparam>
        /// <param name="client"></param>
        /// <param name="functionName"></param>
        /// <param name="payloadObject"></param>
        /// <returns></returns>
        public static Task<InvokeResponse> InvokeRequestAsync<T>(this IAmazonLambda client, string functionName, T payloadObject)
        {
            var payload = JsonSerializer.Serialize(payloadObject);
            var invokeRequest = new InvokeRequest
            {
                FunctionName = functionName,
                InvocationType = InvocationType.RequestResponse,
                Payload = payload
            };
            return client.InvokeAsync(invokeRequest);
        }
    }
}
