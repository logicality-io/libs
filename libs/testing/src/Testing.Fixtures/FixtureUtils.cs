using System;
using System.Net;
using System.Net.Sockets;

namespace Logicality.Testing.Fixtures
{
    public static class FixtureUtils
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
    }
}
