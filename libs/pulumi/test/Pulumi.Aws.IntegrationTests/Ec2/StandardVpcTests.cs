using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Logicality.Pulumi.Automation;
using Pulumi.Automation;
using Pulumi.Aws;
using Shouldly;

namespace Logicality.Pulumi.Aws.Ec2;

public class StandardVpcTests 
{
    private readonly AmazonEC2Client _ec2Client;

    public StandardVpcTests()
    {
        var config = new AmazonEC2Config
        {
            RegionEndpoint = RegionEndpoint.EUWest1
        };
        _ec2Client = new AmazonEC2Client(config);
    }

    public async Task CanDeploy()
    {
        WorkspaceStack stack = null;
        try
        {

            // Arrange
            stack = await InitializeStack(() =>
            {
                var args = new StandardVpcArgs
                {
                    CidrBlockSegment = 1
                };
                var vpc = new StandardVpc("test-vpc", args);

                return new Dictionary<string, object>
                {
                    {"VpcId", vpc.Vpc.Apply(v => v.Id)}
                };
            });

            // Act
            var upResult = await stack.UpAsync(new UpOptions
            {
                OnStandardOutput = s => Console.WriteLine($"stdout:{s}"),
                OnStandardError  = s => Console.WriteLine($"stderr:{s}"),
            });

            // Assert
            var expectedVpcId        = (string) upResult.Outputs["VpcId"].Value;
            var describeVpcsResponse = await _ec2Client.DescribeVpcsAsync();
            describeVpcsResponse.Vpcs.Any(v => v.VpcId == expectedVpcId).ShouldBeTrue();
        }
        finally
        {
            if (stack != null)
            {
                // Cleanup
                var destroyResult = await stack.DestroyAsync();
                Console.WriteLine(destroyResult.StandardOutput);
            }
        }
    }

    private static async Task<WorkspaceStack> InitializeStack(Func<IDictionary<string, object?>> program)
    {
        var pulumiFn    = PulumiFn.Create(program);
        var projectName = $"{typeof(StandardVpcTests)}-{nameof(CanDeploy)}";
        var stackName   = "test";
        var stackArgs   = new InlineProgramArgs(projectName, stackName, pulumiFn);
        stackArgs.ConfigureForLocalBackend();
        var stack = await LocalWorkspace.CreateOrSelectStackAsync(stackArgs);
        await stack.Workspace.InstallPluginAsync<Provider>();
        await stack.SetConfigAsync("aws:region", new ConfigValue("eu-west-1"));

        return stack;
    }
}