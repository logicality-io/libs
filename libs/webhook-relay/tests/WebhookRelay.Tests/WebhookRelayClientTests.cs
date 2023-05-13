using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit.Abstractions;

namespace Logicality.WebhookRelay;

public class WebhookRelayClientTests
{
    private readonly ServiceProvider _services;
    private readonly string          _webhookUrl;

    public WebhookRelayClientTests(ITestOutputHelper outputHelper)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets<WebhookRelayClientTests>()
            .Build();
        
        var tokenKey = configuration["WebhookRelayTokenKey"];
        if (string.IsNullOrWhiteSpace(tokenKey))
            throw new InvalidOperationException(
                "WebhookRelayTokenKey is not set. (Did you forget to set it in your user secrets or environment?)");

        var tokenSecret = configuration["WebhookRelayTokenSecret"];
        if (string.IsNullOrWhiteSpace(tokenSecret))
        {
            throw new InvalidOperationException(
                "WebhookRelayTokenKey is not set. (Did you forget to set it in your user secrets or environment?)");
        }

        var webhookUrl = configuration["WebhookUrl"];
        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new InvalidOperationException(
                "WebhookRelayTokenKey is not set. (Did you forget to set it in your user secrets or environment?)");
        }
        _webhookUrl = webhookUrl;

        _services = new ServiceCollection()
            .AddLogging(builder => builder.AddXUnit(outputHelper))
            .AddSingleton<WebhookRelayClient>()
            .AddSingleton<Channel<WebhookMessage>>(_ => Channel.CreateBounded<WebhookMessage>(1000))
            .AddSingleton<ChannelWriter<WebhookMessage>>(sp => sp.GetRequiredService<Channel<WebhookMessage>>().Writer)
            .AddSingleton<ChannelReader<WebhookMessage>>(sp => sp.GetRequiredService<Channel<WebhookMessage>>().Reader)
            .AddSingleton(new WebhookRelayClientConfiguration(tokenKey, tokenSecret, new[] { "client-test" }))
            .BuildServiceProvider();
    }

    [Fact]
    public async Task Can_connect_authenticate_and_receive_webhook_messages()
    {
        var channelReader = _services.GetRequiredService<ChannelReader<WebhookMessage>>();
        var client  = _services.GetRequiredService<WebhookRelayClient>();
        client.Start();
        await client.WaitForState(ClientState.HandlingMessages, TimeSpan.FromSeconds(5));

        await PostWebhookMessage(new Event("1", "created"));

        var receivedMessage = await channelReader.ReadAsync();

        receivedMessage.Method.ShouldBe("POST");
        receivedMessage.Query.ShouldBeEmpty();
        receivedMessage.Body.ShouldBe("{\"id\":\"1\",\"type\":\"created\"}");
        receivedMessage.Headers.ShouldContainKey("Content-Type");
        receivedMessage.Meta.BucketName.ShouldBe("client-test");
        receivedMessage.Meta.InputName.ShouldNotBeNullOrWhiteSpace();
        receivedMessage.Meta.Id.ShouldNotBeNullOrWhiteSpace();

        await client.DisposeAsync();
    }

    private async Task PostWebhookMessage(Event @event)
    {
        var applicationJsonMediaType = new MediaTypeWithQualityHeaderValue("application/json");
        var client = new HttpClient
        {
            BaseAddress = new Uri(_webhookUrl),
        };
        client.DefaultRequestHeaders.Accept.Add(applicationJsonMediaType);

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var jsonData = JsonSerializer.Serialize(@event, serializerOptions);
        var content  = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("", content: content);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    public record Event(string Id, string Type);
}