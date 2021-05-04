namespace Logicality.Pulumi.Aws.Ec2
{
    public class StandardVpcArgs
    {
        public StandardVpcArgs(int cidrBlockSegment)
        {
            CidrBlockSegment = cidrBlockSegment;
        }

        public int CidrBlockSegment { get; }
    }
}