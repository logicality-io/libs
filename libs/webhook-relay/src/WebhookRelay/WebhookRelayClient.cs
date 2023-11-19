using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Stateless;

namespace Logicality.WebhookRelay;

public sealed class WebhookRelayClient : IAsyncDisposable
{
    private const           int  ReceiveChunkSize = 1024;

    private readonly ILogger<WebhookRelayClient>              _logger;
    private readonly CancellationTokenSource                  _stoppingToken   = new();
    private          ClientWebSocket                          _webSocketClient = null!;
    private readonly JsonSerializerOptions                    _jsonSerializerOptions;
    private readonly StateMachine<ClientState, ClientTrigger> _stateMachine;

    public WebhookRelayClient(
        WebhookRelayClientConfiguration configuration,
        ChannelWriter<WebhookMessage>   channelWriter,
        ILogger<WebhookRelayClient>     logger)
    {
        _logger   = logger   ?? throw new ArgumentNullException(nameof(logger));

        _stateMachine = new StateMachine<ClientState, ClientTrigger>(ClientState.Disconnected);
        _stateMachine
            .Configure(ClientState.Disconnected)
            .Permit(ClientTrigger.Connect, ClientState.Connecting);

        _stateMachine
            .Configure(ClientState.Connecting)
            .OnEntryAsync(async () =>
            {
                // TODO polly
                try
                {
                    _webSocketClient = new ClientWebSocket();
                    await _webSocketClient.ConnectAsync(configuration.Uri, _stoppingToken.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, ex.Message);
                    await _stateMachine.FireAsync(ClientTrigger.ConnectFailed);
                }
                await _stateMachine.FireAsync(ClientTrigger.ConnectCompleted);
            })
            .Permit(ClientTrigger.ConnectCompleted, ClientState.Connected)
            .Permit(ClientTrigger.ConnectFailed, ClientState.Disposed);

        _stateMachine
            .Configure(ClientState.Connected)
            .OnEntryAsync(async () =>
            {
                await _stateMachine.FireAsync(ClientTrigger.Authenticate);
            })
            .Permit(ClientTrigger.Authenticate, ClientState.Authenticating);

        _stateMachine
            .Configure(ClientState.Authenticating)
            .OnEntryAsync(async () =>
            {
                var authenticate = new Authenticate(
                    configuration.TokenKey,
                    configuration.TokenSecret);
                await SendMessage(authenticate, _stoppingToken.Token);
                var (closed, message) = await ReceiveMessage();
                if (closed)
                {
                    await _stateMachine.FireAsync(ClientTrigger.ConnectionLost);
                    return;
                }

                var jsonDocument = JsonDocument.Parse(message);
                var messageType  = jsonDocument.RootElement.GetProperty("type").GetString();
                if (messageType == "status")
                {
                    var status = jsonDocument.RootElement.GetProperty("status").GetString();
                    switch (status)
                    {
                        case "authenticated":
                            await _stateMachine.FireAsync(ClientTrigger.AuthenticateSucceeded);
                            //await SendMessage(new Subscribe { Buckets = buckets.ToArray() }, _stoppingToken.Token);
                            return;
                        case "unauthorized":
                            logger.LogCritical("Authentication failed.");
                            await _stateMachine.FireAsync(ClientTrigger.AuthenticationFailed);
                            return;
                        default:
                            logger.LogCritical("Authentication failed. Unexpected status in response: {status}",
                                status);
                            await _stateMachine.FireAsync(ClientTrigger.AuthenticationFailed);
                            return;
                    }
                }

                logger.LogCritical("Authentication failed. Unexpected type in response: {type}", messageType);
                await _stateMachine.FireAsync(ClientTrigger.AuthenticationFailed);
            })
            .Permit(ClientTrigger.AuthenticateSucceeded, ClientState.Authenticated)
            .Permit(ClientTrigger.AuthenticationFailed, ClientState.Disposed)
            .Permit(ClientTrigger.ConnectionLost, ClientState.Connecting);

        _stateMachine
            .Configure(ClientState.Authenticated)
            .OnEntryAsync(async () =>
            {
                await _stateMachine.FireAsync(ClientTrigger.Subscribe);
            })
            .Permit(ClientTrigger.Subscribe, ClientState.Subscribing)
            .Permit(ClientTrigger.ConnectionLost, ClientState.Disposed);

        _stateMachine
            .Configure(ClientState.Subscribing)
            .OnEntryAsync(async () =>
            {
                var buckets   = configuration.Buckets.ToArray();
                var subscribe = new Subscribe(buckets);
                await SendMessage(subscribe, _stoppingToken.Token);
                var (closed, message) = await ReceiveMessage();
                if (closed)
                {
                    await _stateMachine.FireAsync(ClientTrigger.ConnectionLost);
                    return;
                }
                var jsonDocument = JsonDocument.Parse(message);
                var messageType  = jsonDocument.RootElement.GetProperty("type").GetString();
                if (messageType == "status")
                {
                    var status = jsonDocument.RootElement.GetProperty("status").GetString();
                    switch (status)
                    {
                        case "subscribed":
                            await _stateMachine.FireAsync(ClientTrigger.SubscribeCompleted);
                            //await SendMessage(Pong.Instance, _stoppingToken.Token);
                            return;
                        default:
                            logger.LogCritical("Unrecognized status: {status}", status);
                            await _stateMachine.FireAsync(ClientTrigger.Dispose);
                            return;
                    }
                }
                logger.LogCritical("Authentication failed. Unexpected type in response: {type}", messageType);
                await _stateMachine.FireAsync(ClientTrigger.Dispose);

            })
            .Permit(ClientTrigger.SubscribeCompleted, ClientState.Subscribed)
            .Permit(ClientTrigger.ConnectionLost, ClientState.Connecting);

        _stateMachine
            .Configure(ClientState.Subscribed)
            .OnEntryAsync(async () =>
            {
                await _stateMachine.FireAsync(ClientTrigger.HandleMessages);
            })
            .Permit(ClientTrigger.HandleMessages, ClientState.HandlingMessages)
            .Permit(ClientTrigger.Dispose, ClientState.Disposed);

        _stateMachine
            .Configure(ClientState.HandlingMessages)
            .OnEntryAsync(async () =>
            {
                while (!_stoppingToken.IsCancellationRequested)
                {
                    var (closed, message) = await ReceiveMessage();
                    if (closed)
                    {
                        await _stateMachine.FireAsync(ClientTrigger.ConnectionLost);
                        return;
                    }

                    var jsonDocument = JsonDocument.Parse(message);
                    var messageType  = jsonDocument.RootElement.GetProperty("type").GetString();
                    if (messageType == "status")
                    {
                        var status = jsonDocument.RootElement.GetProperty("status").GetString();
                        switch (status)
                        {
                            case "ping":
                                await SendMessage(Pong.Instance, _stoppingToken.Token);
                                continue;
                            case "unauthorized":
                                logger.LogCritical("Authentication failed.");
                                await _stateMachine.FireAsync(ClientTrigger.AuthenticationFailed);
                                break;
                            default:
                                logger.LogCritical("Unrecognized status: {status}", status);
                                await _stateMachine.FireAsync(ClientTrigger.Dispose);
                                break;
                        }
                    }

                    if(messageType == "webhook")
                    {
                        var webhookMessage = JsonSerializer.Deserialize<WebhookMessage>(message, _jsonSerializerOptions);
                        await channelWriter.WriteAsync(webhookMessage!, _stoppingToken.Token);
                        continue;
                    }

                    logger.LogCritical("Unrecognized type: {type}", messageType);
                    await _stateMachine.FireAsync(ClientTrigger.Dispose);
                    return;
                }
            })
            .Permit(ClientTrigger.ConnectionLost, ClientState.Connecting)
            .Permit(ClientTrigger.Dispose, ClientState.Disposed);

        _stateMachine.OnTransitioned(transition =>
        {
            OnClientStateChanged(transition.Destination);
            logger.LogInformation("Transitioned from {source} to {destination} because of {trigger}", transition.Source, transition.Destination, transition.Trigger);
        });

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition      = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
        };
    }

    private async Task<(bool Closed, string Message)> ReceiveMessage()
    {
        // TODO make less allocaty
        var                    buffer = new byte[ReceiveChunkSize];
        var                    bytes  = new List<byte>();
        WebSocketReceiveResult result;
        do
        {
            result = await _webSocketClient.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                _stoppingToken.Token);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocketClient.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    string.Empty,
                    CancellationToken.None);
                return (false, string.Empty);
            }
            bytes.AddRange(buffer[..result.Count]);
        } while (!result.EndOfMessage);
        var message = Encoding.UTF8.GetString(bytes.ToArray());

        _logger.LogTrace("Received message {message}", message);
        return (false, message);
    }

    public async void Start() => await _stateMachine.FireAsync(ClientTrigger.Connect);

    public event EventHandler<ClientState>? StateChanged;

    private async Task SendMessage(object message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending message {messageType}", message.GetType());
        var json  = JsonSerializer.Serialize(message, _jsonSerializerOptions);
        var bytes = Encoding.UTF8.GetBytes(json);
        await _webSocketClient.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            cancellationToken);
    }

    private record Authenticate(string Key, string Secret)
    {
        public string Action { get; } = "auth";
    }

    private record Subscribe(string[] Buckets)
    {
        public string Action { get; } = "subscribe";
    }

    private class Pong
    {
        internal static readonly Pong Instance = new();

        public string Action { get; } = "pong";
    }

    public async ValueTask DisposeAsync()
    {
        await _webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        // wait for state machine to transition to closed.
    }

    private void OnClientStateChanged(ClientState e) => StateChanged?.Invoke(this, e);
}