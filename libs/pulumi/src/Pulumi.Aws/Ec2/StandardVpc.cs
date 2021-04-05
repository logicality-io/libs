using Pulumi;
using Pulumi.Aws.Ec2;

namespace Logicality.Pulumi.Aws.Ec2
{
    /// <summary>
    /// A cut down version of Pulumi's Awsx's VPC.
    /// </summary>
    public class StandardVpc : ComponentResource
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="options"></param>
        public StandardVpc(string name, StandardVpc args, ComponentResourceOptions? options = null)
            : base("logicality:aws:ec2:StandardVpc", name, options)
        {
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StandardVpcArgs
    {
        /// <summary>
        /// The CIDR block for the VPC. Default is 10.0.0.0/16
        /// </summary>
        [Input("cidrBlock", required: true)]
        public Input<string> CidrBlock { get; set; } = "10.0.0.0/16";

        /// <summary>
        /// A boolean flag to enable/disable DNS hostnames in the VPC. Defaults false.
        /// </summary>
        [Input("enableDnsHostnames")]
        public Input<bool> EnableDnsHostnames { get; set; } = false;

        /// <summary>
        /// A boolean flag to enable/disable DNS support in the VPC. Defaults true.
        /// </summary>
        [Input("enableDnsSupport")]
        public Input<bool> EnableDnsSupport { get; set; } = true;

        /// <summary>
        /// A tenancy option for instances launched into the VPC. Default is `default`, which
        /// makes your instances shared on the host. Using either of the other options (`dedicated` or `host`) costs at least $2/hr.
        /// </summary>
        [Input("instanceTenancy")]
        public Input<string>? InstanceTenancy { get; set; }

        /// <summary>
        /// A map of tags to assign to the resource.
        /// </summary>
        [Input("tags")]
        public InputMap<string> Tags { get; set; } = new InputMap<string>();
    }
}
