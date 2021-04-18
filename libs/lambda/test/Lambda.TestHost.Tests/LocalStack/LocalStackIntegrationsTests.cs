using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.SQS.Model;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Logicality.AWS.Lambda.TestHost.LocalStack
{
    public class LocalStackIntegrationsTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public LocalStackIntegrationsTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task With_lambda_service_and_LAMBDA_FORWARD_URL_then_should_invoke() 
        {
            await using var fixture = await LocalStackFixture.Create(_outputHelper);

            // 1. Arrange: Create dummy lambda function in localstack
            var functionInfo = fixture.LambdaTestHost.Settings.Functions.First().Value;
            var createFunctionRequest = new CreateFunctionRequest
            {
                Handler = "dummy-handler", // ignored
                FunctionName = functionInfo.Name, // must match
                Role = "arn:aws:iam::123456789012:role/foo", // must be specified
                Code = new FunctionCode
                {
                    ZipFile = new MemoryStream() // must be specified but is ignored
                }
            };
            await fixture.LambdaClient.CreateFunctionAsync(createFunctionRequest);

            // 2. Act: Call lambda Invoke API
            var invokeRequest = new InvokeRequest
            {
                FunctionName = functionInfo.Name,
                Payload = "{\"Data\":\"Bar\"}",
            };
            var invokeResponse = await fixture.LambdaClient.InvokeAsync(invokeRequest);

            // 3. Assert: Check payload was forwarded
            invokeResponse.HttpStatusCode.ShouldBe(HttpStatusCode.OK);
            invokeResponse.FunctionError.ShouldBeNullOrEmpty();
            var responsePayload = Encoding.UTF8.GetString(invokeResponse.Payload.ToArray());
            responsePayload.ShouldStartWith("{\"Reverse\":\"raB\"}");
        }

        [Fact]
        public async Task Should_invoke_with_sqs_event_source_mapping()
        {
            await using var fixture = await LocalStackFixture.Create(_outputHelper);

            // 1.1 Arrange: Create dummy lambda function in localstack
            var functionInfo = fixture.LambdaTestHost.Settings.Functions.Last().Value;
            var createFunctionRequest = new CreateFunctionRequest
            {
                Handler = "dummy-handler", // ignored
                FunctionName = functionInfo.Name, // must match
                Role = "arn:aws:iam::123456789012:role/foo", // must be specified
                Code = new FunctionCode
                {
                    ZipFile = new MemoryStream() // must be specified but is ignored
                }
            };
            await fixture.LambdaClient.CreateFunctionAsync(createFunctionRequest);

            // 1.2 Arrange: Create queue and event source mapping
            var queueName = "test-queue";
            var createQueueResponse = await fixture.SQSClient.CreateQueueAsync(queueName);
            var createEventSourceMappingRequest = new CreateEventSourceMappingRequest
            {
                FunctionName = functionInfo.Name,
                EventSourceArn = $"arn:aws:sqs:eu-west-1:123456789012:{queueName}",
                BatchSize = 1,
                Enabled = true,
            };
            var createEventSourceMappingResponse = await fixture.LambdaClient.CreateEventSourceMappingAsync(createEventSourceMappingRequest);

            // 2. Act: send message to queue
            await fixture.SQSClient.SendMessageAsync(createQueueResponse.QueueUrl, "hello");

            await Task.Delay(10000);
        }
    }
}
 