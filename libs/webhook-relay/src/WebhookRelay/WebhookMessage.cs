namespace Logicality.WebhookRelay;

public record WebhookMessage(
    Metadata                     Meta,
    Dictionary<string, string[]> Headers,
    string                       Query,
    string                       Body,
    string                       Method);
