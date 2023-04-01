namespace Logicality.WebhookRelay;

public enum ClientState
{
    Disconnected,
    Connecting,
    Connected,
    Authenticating,
    Authenticated,
    Subscribing,
    Subscribed,
    HandlingMessages,
    Disposed,
}