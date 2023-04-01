using System.Runtime.Serialization;

namespace Logicality.WebhookRelay;

public class Metadata
{
    public string Id { get; set; }

    [DataMember(Name = "bucket_id")]
    public string BucketId { get; set; }

    [DataMember(Name = "bucket_name")]
    public string BucketName { get; set; }

    [DataMember(Name = "input_id")]
    public string InputId { get; set; }

    [DataMember(Name = "input_name")]
    public string InputName { get; set; }

    [DataMember(Name = "output_name")]
    public string OutputName { get; set; }

    [DataMember(Name = "output_destination")]
    public string OutputDestination { get; set; }
}