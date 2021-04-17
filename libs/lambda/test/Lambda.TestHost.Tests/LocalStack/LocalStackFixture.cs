using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Runtime;
using Amazon.SQS;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Logicality.AWS.Lambda.TestHost.LocalStack
{
    public class LocalStackFixture : IAsyncDisposable
    {
        private readonly IContainerService _localStack;
        private readonly ITestOutputHelper _outputHelper;
        private const int ContainerPort = 4566;

        public LocalStackFixture(LambdaTestHost lambdaTestHost,
            IContainerService localStack,
            Uri serviceUrl,
            ITestOutputHelper outputHelper)
        {
            ServiceUrl = serviceUrl;
            LambdaTestHost = lambdaTestHost;
            _localStack = localStack;
            _outputHelper = outputHelper;

            AWSCredentials = new BasicAWSCredentials("not", "used");

            LambdaClient = new AmazonLambdaClient(AWSCredentials, new AmazonLambdaConfig
            {
                ServiceURL = ServiceUrl.ToString()
            });

            SQSClient = new AmazonSQSClient(AWSCredentials, new AmazonSQSConfig
            {
                ServiceURL = ServiceUrl.ToString()
            });
        }


        public Uri ServiceUrl { get; }

        public LambdaTestHost LambdaTestHost { get; }

        public AWSCredentials AWSCredentials { get; }

        public IAmazonLambda LambdaClient { get; }

        public IAmazonSQS SQSClient { get; }

        public static async Task<LocalStackFixture> Create(ITestOutputHelper outputHelper)
        {
            var webHostUrl = FixtureUtils.IsRunningInContainer
                ? FixtureUtils.GetLocalIPAddress()
                : "127.0.0.1";

            // Runs a the Lambda TestHost (invoke api) on a random port
            var settings = new LambdaTestHostSettings(() => new TestLambdaContext())
            {
                WebHostUrl = $"http://{webHostUrl}:0",
                ConfigureLogging = logging =>
                {
                    logging.AddXUnit(outputHelper);
                    logging.SetMinimumLevel(LogLevel.Debug);
                }
            };
            settings.AddFunction(new LambdaFunctionInfo(
                nameof(SimpleLambdaFunction),
                typeof(SimpleLambdaFunction),
                nameof(SimpleLambdaFunction.FunctionHandler)));
            settings.AddFunction(new LambdaFunctionInfo(
                nameof(SQSLambdaFunction),
                typeof(SQSLambdaFunction),
                nameof(SQSLambdaFunction.FunctionHandler)));
            var lambdaTestHost = await LambdaTestHost.Start(settings);

            var lambdaInvokeEndpoint = FixtureUtils.GetLambdaInvokeEndpoint(outputHelper, lambdaTestHost);

            var localStackBuilder = new Builder()
                .UseContainer()
                .WithName($"lambda-testhost-localstack-{Guid.NewGuid()}")
                .UseImage("localstack/localstack:latest")
                .WithEnvironment(
                    "SERVICES=lambda,sqs",
                    "LS_LOG=debug",
                    $"LAMBDA_FORWARD_URL={lambdaInvokeEndpoint}")
                .ExposePort(0, ContainerPort);
            var localStack = localStackBuilder.Build().Start();

            var exposedPort = localStack
                .GetConfiguration()
                .NetworkSettings
                .Ports.First()
                .Value.First()
                .HostPort;

            var localstackServiceUrl = new UriBuilder($"http://localhost:{exposedPort}");

            if (FixtureUtils.IsRunningInContainer)
            {
                var host = localStack
                    .GetConfiguration()
                    .NetworkSettings
                    .IPAddress;

                localstackServiceUrl.Host = host;
                localstackServiceUrl.Port = ContainerPort;
            }

            outputHelper.WriteLine($"Using localstackServiceUrl={localstackServiceUrl}");
            return new LocalStackFixture(lambdaTestHost, localStack, localstackServiceUrl.Uri, outputHelper);
        }
       
        public async ValueTask DisposeAsync()
        {
            await LambdaTestHost.DisposeAsync();

            var hosts = new Hosts().Discover();
            var docker = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

            await Task.Delay(1000);
            _outputHelper.WriteLine("--- Begin container logs ---");
            using (var logs = docker?.Host.Logs(_localStack.Id, certificates: docker.Certificates))
            {
                var line = logs!.Read();
                while (line != null)
                {
                    _outputHelper.WriteLine(line);
                    line = logs!.Read();
                }
            }
            _outputHelper.WriteLine("--- End container logs ---");

            _localStack.RemoveOnDispose = true;
            _localStack.Dispose();
        }
    }
}