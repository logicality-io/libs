using System;
using System.Net;
using System.Net.Sockets;
using Xunit.Abstractions;

namespace Logicality.AWS.Lambda.TestHost
{
    internal static class FixtureUtils
    {
        static FixtureUtils()
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
            IsRunningInContainer = env != null && env.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsRunningInContainer { get; }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GetLambdaInvokeEndpoint(ITestOutputHelper outputHelper, LambdaTestHost lambdaTestHost)
        {
            var lambdaForwardUrlBuilder = new UriBuilder(lambdaTestHost.ServiceUrl);
            if (!IsRunningInContainer)
            {
                lambdaForwardUrlBuilder.Host = "host.docker.internal";
            }

            var lambdaForwardUrl = lambdaForwardUrlBuilder.ToString();
            //  Remove trailing slash as localstack does string concatenation resulting in "//".
            lambdaForwardUrl = lambdaForwardUrl.Remove(lambdaForwardUrl.Length - 1);
            outputHelper.WriteLine($"Using LAMBDA_FALLBACK_URL={lambdaForwardUrl}");
            return lambdaForwardUrl;
        }
    }
}