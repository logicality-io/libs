namespace Logicality.WebhookRelay;

public class WebhookMessage
{
    public Metadata Meta { get; set; }

    public Dictionary<string, string[]> Headers { get; set; }

    public string Query { get; set; }

    public string Body { get; set; }

    public string Method { get; set; }
}