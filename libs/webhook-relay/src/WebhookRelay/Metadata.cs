using System.Text.Json.Serialization;

namespace Logicality.WebhookRelay;

public class Metadata
{
    [JsonPropertyName("id")]

    public string Id { get; set; }

    [JsonPropertyName("bucket_id")]
    public string BucketId { get; set; }

    [JsonPropertyName("bucket_name")]
    public string BucketName { get; set; }

    [JsonPropertyName("input_id")]
    public string InputId { get; set; }

    [JsonPropertyName("input_name")]
    public string InputName { get; set; }

    [JsonPropertyName("output_name")]
    public string OutputName { get; set; }

    [JsonPropertyName("output_destination")]
    public string OutputDestination { get; set; }
}