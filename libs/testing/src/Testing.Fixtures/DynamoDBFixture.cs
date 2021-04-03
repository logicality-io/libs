using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Common;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Polly;

namespace Logicality.Testing.Fixtures
{
    public class DynamoDBFixture : IDisposable
    {
        private const int ContainerPort = 8000;
        private readonly IContainerService _containerService;

        private DynamoDBFixture(IContainerService containerService, AmazonDynamoDBClient dynamoDBClient)
        {
            DynamoDBClient = dynamoDBClient;
            _containerService = containerService;
        }

        public static Task<DynamoDBFixture> Create(string containerNamePrefix, string imageTag = "latest", int port = 0)
        {
            return Task.Run(async () =>
            {
                IContainerService containerService;
                var name = $"{containerNamePrefix}-dynamodb";
                try
                {
                    containerService = new Builder()
                        .UseContainer()
                        .WithName($"{containerNamePrefix}-dynamodb")
                        .UseImage($"amazon/dynamodb-local:{imageTag}")
                        .ReuseIfExists()
                        .Command("", "-jar", "DynamoDBLocal.jar", "-inMemory", "-sharedDb")
                        .ExposePort(port, ContainerPort)
                        .WaitForPort($"{ContainerPort}/tcp", TimeSpan.FromSeconds(5))
                        .Build();
                    containerService.Start();
                }
                catch (FluentDockerException ex) when (ex.Message.Contains("Error response from daemon: Conflict"))
                {
                    // This can happen in a container startup race condition and parallel tests.
                    // Assume the container is already running.

                    var hosts = new Hosts().Discover();
                    var docker = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");
                    
                    var waitAndRetry = Polly.Policy.Handle<FluentDockerException>()
                        .WaitAndRetry(10, _ => TimeSpan.FromMilliseconds(500));

                    containerService = waitAndRetry.Execute(() =>
                    {
                        return docker!.GetContainers()
                            .Single(c => c.Name == name)
                            .WaitForPort($"{ContainerPort}/tcp", 5000);
                    });
                }

                var config = containerService.GetConfiguration();
                var networkSettings = config.NetworkSettings;
                var exposedPort = networkSettings.Ports.First();
                var hostPort = exposedPort.Value.First().HostPort;

                var serviceUrl = new UriBuilder($"http://localhost:{hostPort}");

                if (FixtureUtils.IsRunningInContainer)
                {
                    // When tests are running in container, the networking setup is different.
                    // Instead of host -> container, we have container -> container so
                    // localhost won't work as host networking does not apply
                    var host = containerService
                        .GetConfiguration()
                        .NetworkSettings
                        .IPAddress;

                    serviceUrl.Host = host;
                    serviceUrl.Port = ContainerPort;
                }

                var dynamoDBConfig = new AmazonDynamoDBConfig
                {
                    ServiceURL = serviceUrl.ToString()
                };
                // If not specified, AWSSDK will try to load credentials from profile which won't exist in container.
                var credentials = new BasicAWSCredentials("not", "used");
                var dynamoDBClient = new AmazonDynamoDBClient(credentials, dynamoDBConfig);

                return new DynamoDBFixture(containerService, dynamoDBClient);
            });
        }

        public IAmazonDynamoDB DynamoDBClient { get; }

        /// <summary>
        /// Disposes and shuts down the container.
        /// </summary>
        public void Dispose()
        {
            _containerService.Dispose();
            DynamoDBClient.Dispose();
        }
    }
}
