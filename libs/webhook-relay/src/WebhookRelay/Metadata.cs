using System.Text.Json.Serialization;

namespace Logicality.WebhookRelay;

public class Metadata
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("bucket_id")]
    public string BucketId { get; set; } = null!;

    [JsonPropertyName("bucket_name")]
    public string BucketName { get; set; } = null!;

    [JsonPropertyName("input_id")]
    public string InputId { get; set; } = null!;

    [JsonPropertyName("input_name")]
    public string InputName { get; set; } = null!;

    [JsonPropertyName("output_name")]
    public string OutputName { get; set; } = null!;

    [JsonPropertyName("output_destination")]
    public string OutputDestination { get; set; } = null!;
}