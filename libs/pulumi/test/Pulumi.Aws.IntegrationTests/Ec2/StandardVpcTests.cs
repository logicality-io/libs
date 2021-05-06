using System.Threading.Tasks;
using Logicality.Pulumi.Automation;
using Pulumi;
using Pulumi.Automation;
using Pulumi.Aws;
using Pulumi.Aws.S3;

namespace Logicality.Pulumi.Aws.Ec2
{
    public class StandardVpcTests
    {
        public async Task Can_deploy_vpc()
        {
            var workspaceOptions = new LocalWorkspaceOptions
            {
                Program = PulumiFn.Create<MyStack>(),
            }.ConfigureForLocalBackend(nameof(Can_deploy_vpc));

            var localWorkspace = await LocalWorkspace.CreateAsync(workspaceOptions);
            await localWorkspace.InstallPluginAsync<Provider>();

            var workspaceStack = await WorkspaceStack.CreateOrSelectAsync("test", localWorkspace);
            var previewResult = await workspaceStack.PreviewAsync();

            await workspaceStack.DestroyAsync();
        }

        private void StandardVpcTestStack()
        {
            // Create an AWS resource (S3 Bucket)
            var bucket = new Bucket("my-bucket");

            // Export the name of the bucket
            //this.BucketName = bucket.Id;
        }
    }

    public class MyStack : Stack
    {
        public MyStack()
        {
            // Create an AWS resource (S3 Bucket)
            var bucket = new Bucket("my-bucket");

            // Export the name of the bucket
            this.BucketName = bucket.Id;
        }

        [Output]
        public Output<string> BucketName { get; set; }
    }
}
