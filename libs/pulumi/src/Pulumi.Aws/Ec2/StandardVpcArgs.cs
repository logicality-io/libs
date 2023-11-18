using Pulumi;

namespace Logicality.Pulumi.Aws.Ec2;

public class StandardVpcArgs
{
    [Input("cidrBlockSegment")]
    public int CidrBlockSegment { get; set; }
}