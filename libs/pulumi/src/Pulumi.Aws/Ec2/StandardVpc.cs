using System.Threading.Tasks;
using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Ec2;

namespace Logicality.Pulumi.Aws.Ec2
{
    /// <summary>
    /// Create a standard VPC across 2 availability zoned with
    /// - 2 private subnets
    /// - 2 public subnets
    /// - Nat gateways with an elastic IPs attached to public subnets
    /// - Internet gateway
    /// - Appropriate route table
    ///
    /// CIDR blocs are in the /16 range. A cidr block segment can be supplied
    /// so multiple VPCs can have non-overlaping cidr blocks.
    /// </summary>
    public class StandardVpc : ComponentResource
    {
        private readonly StandardVpcArgs _args;
        private readonly ComponentResourceOptions? _options;

        public StandardVpc(string name, StandardVpcArgs args, ComponentResourceOptions? options = null)
            : base("logicality:puluimi:aws:StandardVpc", name, options)
        {
            _args = args;
            _options = options;

            var dataTask = Define();

            var data = Output<VpcData>.Create(dataTask);

            Vpc = data.Apply(d => d.Vpc);
            PrivateSubnet0 = data.Apply(d => d.PrivateSubnet0);
            PrivateSubnet1 = data.Apply(d => d.PrivateSubnet1);
            PublicSubnet0 = data.Apply(d => d.PublicSubnet0);
            PublicSubnet1 = data.Apply(d => d.PublicSubnet1);

            RegisterOutputs();
        }

        private async Task<VpcData> Define()
        {
            var availabilityZonesResult = await GetAvailabilityZones.InvokeAsync(null, new InvokeOptions { Provider = _options?.Provider });

            var azA = availabilityZonesResult.Names[0];
            var azB = availabilityZonesResult.Names[1];

            var vpcResourceName = $"{GetResourceName()}";
            var vpc = new Vpc(
                vpcResourceName,
                new VpcArgs
                {
                    CidrBlock = $"10.{_args.CidrBlockSegment}.0.0/16",
                    Tags = new InputMap<string>
                    {
                        { "Name", vpcResourceName },
                        { "pulumi:ResourceName", vpcResourceName }
                    }
                },
                new CustomResourceOptions
                {
                    Provider = _options?.Provider,
                    Parent = this
                });

            var private0 = AddSubnet(vpcResourceName, vpc, "private", azA, 0, _args.CidrBlockSegment, 0);
            var private1 = AddSubnet(vpcResourceName, vpc, "private", azB, 1, _args.CidrBlockSegment, 64);
            var public0 = AddSubnet(vpcResourceName, vpc, "public", azA, 0, _args.CidrBlockSegment, 128);
            var public1 = AddSubnet(vpcResourceName, vpc, "public", azB, 1, _args.CidrBlockSegment, 192);

            var ig = new InternetGateway(
                $"{vpc.GetResourceName()}-ig",
                new InternetGatewayArgs
                {
                    VpcId = vpc.Id
                },
                new CustomResourceOptions
                {
                    Parent = vpc
                });

            new Route($"{public0.RouteTable.GetResourceName()}-ig", new RouteArgs
            {
                RouteTableId = public0.RouteTable.Id,
                DestinationCidrBlock = "0.0.0.0/0",
                GatewayId = ig.Id
            }, new CustomResourceOptions
            {
                Parent = public0.RouteTable
            });

            new Route($"{public1.RouteTable.GetResourceName()}-ig", new RouteArgs
            {
                RouteTableId = public1.RouteTable.Id,
                DestinationCidrBlock = "0.0.0.0/0",
                GatewayId = ig.Id
            }, new CustomResourceOptions
            {
                Parent = public1.RouteTable
            });


            var eip0 = new Eip(
                $"{public0.Subnet.GetResourceName()}-eip",
                new EipArgs
                {
                    Vpc = true,
                    Tags = new InputMap<string>
                    {
                        { "Name" ,  $"{public0.Subnet.GetResourceName()}-eip" }
                    }
                },
                new CustomResourceOptions
                {
                    Parent = public0.Subnet
                });

            var natGateway0 = new NatGateway(
                $"{public0.Subnet.GetResourceName()}-ng",
                new NatGatewayArgs
                {
                    SubnetId = public0.Subnet.Id,
                    AllocationId = eip0.Id,
                    Tags = new InputMap<string>
                    {
                        { "Name", $"{public0.Subnet.GetResourceName()}-ng" }
                    }
                },
                new CustomResourceOptions
                {
                    Parent = public0.Subnet
                });

            new Route($"{private0.Subnet.GetResourceName()}-nat-0", new RouteArgs
            {
                RouteTableId = private0.RouteTable.Id,
                DestinationCidrBlock = "0.0.0.0/0",
                NatGatewayId = natGateway0.Id
            }, new CustomResourceOptions
            {
                Parent = private0.Subnet
            });


            var eip1 = new Eip(
                $"{public1.Subnet.GetResourceName()}-eip",
                new EipArgs
                {
                    Vpc = true,
                    Tags = new InputMap<string>
                    {
                        { "Name" ,  $"{public1.Subnet.GetResourceName()}-eip" }
                    }
                },
                new CustomResourceOptions
                {
                    Parent = public1.Subnet
                });

            var natGateway1 = new NatGateway(
                $"{public1.Subnet.GetResourceName()}-ng",
                new NatGatewayArgs
                {
                    SubnetId = public1.Subnet.Id,
                    AllocationId = eip1.Id,
                    Tags = new InputMap<string>
                    {
                        { "Name", $"{public1.Subnet.GetResourceName()}-ng" }
                    }
                },
                new CustomResourceOptions
                {
                    Parent = public1.Subnet
                });

            new Route($"{private1.Subnet.GetResourceName()}-nat-1", new RouteArgs
            {
                RouteTableId = private1.RouteTable.Id,
                DestinationCidrBlock = "0.0.0.0/0",
                NatGatewayId = natGateway1.Id
            }, new CustomResourceOptions
            {
                Parent = private1.Subnet
            });

            return new VpcData
            {
                Vpc = vpc,
                PrivateSubnet0 = private0.Subnet,
                PrivateSubnet1 = private1.Subnet,
                PublicSubnet0 = public0.Subnet,
                PublicSubnet1 = public1.Subnet,
            };
        }

        public Output<Vpc> Vpc { get; }

        public Output<Subnet> PrivateSubnet0 { get; }

        public Output<Subnet> PrivateSubnet1 { get; }

        public Output<Subnet> PublicSubnet0 { get; }

        public Output<Subnet> PublicSubnet1 { get; }

        private (Subnet Subnet, RouteTable RouteTable) AddSubnet(
            string vpcResourceName,
            Vpc vpc,
            string type,
            string availabilityZone,
            int subnetNumber,
            int cidrBlockSegment,
            int cidrPartition)
        {
            var name = $"{vpcResourceName}-{type}-{subnetNumber}";
            var subnet = new Subnet(
                name,
                new SubnetArgs
                {
                    CidrBlock = $"10.{cidrBlockSegment}.{cidrPartition}.0/18",
                    AvailabilityZone = availabilityZone,
                    VpcId = vpc.Id,
                    Tags = new InputMap<string>
                    {
                        { "Name", $"{name}" },
                        { "pulumi:ResourceName", name }
                    }
                },
                new CustomResourceOptions
                {
                    Parent = vpc
                });

            var routeTable = new RouteTable(
                name,
                new RouteTableArgs
                {
                    VpcId = vpc.Id,
                },
                new CustomResourceOptions
                {
                    Parent = subnet
                });

            new RouteTableAssociation(
                name,
                new RouteTableAssociationArgs
                {
                    RouteTableId = routeTable.Id,
                    SubnetId = subnet.Id
                },
                new CustomResourceOptions
                {
                    Parent = subnet
                });

            return (subnet, routeTable);
        }

        private class VpcData
        {
            public Vpc Vpc { get; set; }

            public Subnet PrivateSubnet0 { get; set; }

            public Subnet PrivateSubnet1 { get; set; }

            public Subnet PublicSubnet0 { get; set; }

            public Subnet PublicSubnet1 { get; set; }
        }
    }
}
