using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Pulumi.Aws.S3;
using Xunit;
using Xunit.Abstractions;

namespace Logicality.Pulumi.Aws.Ec2
{
    public class StandardVpcTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public StandardVpcTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void StandardVpcTest()
        {
            _outputHelper.WriteLine(Environment.CurrentDirectory);
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
            }
        }

        public void StandardVpcTestStack()
        {
            var bucket = new Bucket("bucket", new BucketArgs
            {
                BucketName = "temp-bucket",
            });
        }
    }

    public class PulumiProcess
    {
        private PulumiProcess(string output, string error)
        {
            Output = output;
            Error = error;
        }

        public string Output { get; }
        
        public string Error { get; }

        public static PulumiProcess Run(string args, ITestOutputHelper outputHelper)
        {
            var processStartInfo = new ProcessStartInfo("pulumi", args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = new Process
            {
                StartInfo = processStartInfo
            };
            var error = new StringBuilder();
            var output = new StringBuilder();
            process.ErrorDataReceived += (sender, args) =>
            {
                error.AppendLine(args.Data);
            };
            process.OutputDataReceived += (sender, args) =>
            {
                output.AppendLine(args.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            var exitCode = process.ExitCode;
            process.Close();

            outputHelper.WriteLine($"pulumi {args}");
            outputHelper.WriteLine($"Exit code: {exitCode}");
            if (!string.IsNullOrWhiteSpace(output.ToString()))
            {
                outputHelper.WriteLine("-- Output --");
                outputHelper.WriteLine($"{output}");
            }
            if (!string.IsNullOrWhiteSpace(error.ToString()))
            {
                outputHelper.WriteLine("-- Error --");
                outputHelper.WriteLine($"Error: {error}");
            }
            if (exitCode != 0)
            {
                throw new NonZeroExitCodeException(exitCode);

            }

            return new PulumiProcess(output.ToString(), error.ToString());
        }
    }

    public class NonZeroExitCodeException : Exception
    {
        public NonZeroExitCodeException(int exitCode)
            : base($"The process exited with code {exitCode}.") => this.ExitCode = exitCode;

        public int ExitCode { get; }
    }
}
