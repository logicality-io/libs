namespace Logicality.WebhookRelay;

public class WebhookRelayClientConfiguration
{
    public WebhookRelayClientConfiguration(string tokenKey, string tokenSecret, string[] buckets)
    {
        if (string.IsNullOrWhiteSpace(tokenKey))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(tokenKey));
        }

        if (string.IsNullOrWhiteSpace(tokenSecret))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(tokenSecret));
        }

        if (buckets.Length == 0)
        {
            throw new ArgumentException("Value cannot be an empty collection.", nameof(buckets));
        }

        TokenKey    = tokenKey;
        TokenSecret = tokenSecret;
        Buckets     = buckets;
    }

    /// <summary>
    /// The token key to use for authentication.
    /// </summary>
    public string TokenKey { get; }

    /// <summary>
    /// The token secret to use for authentication.
    /// </summary>
    public string TokenSecret { get; }

    /// <summary>
    /// The buckets to subscribe to.
    /// </summary>
    public IReadOnlyCollection<string> Buckets { get; }

    /// <summary>
    /// Uri of the Webhook Relay server. Defaults to https://my.webhookrelay.com/v1/socket
    /// </summary>
    public Uri Uri { get; } = new("wss://my.webhookrelay.com/v1/socket");
};