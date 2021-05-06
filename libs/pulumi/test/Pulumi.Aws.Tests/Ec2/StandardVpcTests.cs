using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Logicality.Pulumi.Automation;
using Pulumi.Automation;
using Pulumi.Aws.S3;
using Xunit;
using Xunit.Abstractions;

namespace Logicality.Pulumi.Aws.Ec2
{
    public class StandardVpcTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _outputHelper;
        private LocalWorkspace _localWorkspace;
        private WorkspaceStack _workspaceStack;

        public StandardVpcTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task StandardVpcTest()
        {
            var previewResult = await _workspaceStack.PreviewAsync();
            /*_outputHelper.WriteLine(Environment.CurrentDirectory);
            _outputHelper.WriteLine(Assembly.GetExecutingAssembly().Location);
            
            var stackName = nameof(StandardVpcTest);
            try
            {
                Environment.SetEnvironmentVariable("PULUMI_CONFIG_PASSPHRASE", "x");
                var type = typeof(StandardVpcTests);
                var methodInfo = type.GetMethod("StandardVpcTestStack");
                _outputHelper.WriteLine(typeof(StandardVpcTests).FullName);
                PulumiProcess.Run($"stack init {stackName} --non-interactive", _outputHelper);
                PulumiProcess.Run($"config set test:type {typeof(StandardVpcTests).FullName} --non-interactive", _outputHelper);
                PulumiProcess.Run($"config set test:method {nameof(StandardVpcTestStack)} --non-interactive", _outputHelper);
                PulumiProcess.Run($"preview --stack {stackName}", _outputHelper);
            }
            catch (NonZeroExitCodeException ex)
            {
                _outputHelper.WriteLine($"Exception: {ex.ExitCode}");
                throw;
            }
            finally
            {
                PulumiProcess.Run($"stack rm {stackName} -y --non-interactive", _outputHelper);
            }*/
        }

        public void StandardVpcTestStack()
        {
            // Create an AWS resource (S3 Bucket)
            var bucket = new Bucket("my-bucket");

            // Export the name of the bucket
            //this.BucketName = bucket.Id;
        }

        public async Task InitializeAsync()
        {
            // This setup expects Pulumi CLI on PATH. TODO: can this be done in a container?
            // Clean this up
            var workspaceOptions = new LocalWorkspaceOptions
            {
                Program = PulumiFn.Create(StandardVpcTestStack),
            }.ConfigureForLocalBackend(nameof(StandardVpcTests));

            _localWorkspace = await LocalWorkspace.CreateAsync(workspaceOptions);
            await _localWorkspace.InstallPluginAsync<global::Pulumi.Aws.Provider>();

            _workspaceStack = await WorkspaceStack.CreateOrSelectAsync("test", _localWorkspace);
        }

        public Task DisposeAsync()
        {
            return _workspaceStack.DestroyAsync();
        }
    }
}
