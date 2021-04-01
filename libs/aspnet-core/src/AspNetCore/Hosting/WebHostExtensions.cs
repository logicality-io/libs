using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Logicality.AspNetCore.Hosting
{
    /// <summary>
    /// A set of extensions over <see cref="IWebHost"/>
    /// </summary>
    public static class WebHostExtensions
    {
        /// <summary>
        /// Gets the server port. Typically used with a server that was started on port 0 and
        /// whose port was assigned by the OS.
        /// </summary>
        /// <param name="webHost">The web</param>
        /// <returns></returns>
        public static int GetServerPort(this IWebHost webHost)
        {
            var address = webHost.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();
            var match = Regex.Match(address, @"^.+:(\d+)$");
            var port = 0;

            if (match.Success)
            {
                port = int.Parse(match.Groups[1].Value);
            }

            return port;
        }
    }
}
