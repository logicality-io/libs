using System;
using System.Threading.Tasks;
using Logicality.Pulumi.Automation;
using Pulumi.Automation;
using Pulumi.Aws;
using Pulumi.Aws.S3;

namespace Logicality.Pulumi.Aws.Ec2
{
    public class StandardVpcTests 
    {
        public async Task Can_deploy()
        {
            var program = PulumiFn.Create(() =>
            {
                var bucket = new Bucket("my-bucket");
            });
            var projectName = $"{typeof(StandardVpcTests)}-{nameof(Can_deploy)}";
            var stackName   = "test";
            var stackArgs   = new InlineProgramArgs(projectName, stackName, program);
            stackArgs.ConfigureForLocalBackend();
            var stack = await LocalWorkspace.CreateOrSelectStackAsync(stackArgs);
            await stack.Workspace.InstallPluginAsync<Provider>();
            await stack.SetConfigAsync("aws:region", new ConfigValue("eu-west-1"));

            var upResult = await stack.UpAsync();

            Console.WriteLine(upResult.StandardOutput);

            await stack.DestroyAsync();
        }
    }
}
