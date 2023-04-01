namespace Logicality.WebhookRelay;

internal enum ClientTrigger
{
    Connect,
    ConnectCompleted,
    ConnectFailed,
    Authenticate,
    AuthenticateSucceeded,
    AuthenticationFailed,
    Subscribe,
    SubscribeCompleted,
    HandleMessages,
    ConnectionLost,
    Dispose
}