using System;
using System.Linq;
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
        /// Gets the addresses the server is listening on.
        /// </summary>
        /// <param name="webHost">The WebHost</param>
        /// <returns>The collection of URI the host is listening on.</returns>
        public static Uri[] GetServerUris(this IWebHost webHost) =>
            webHost
                .ServerFeatures
                .Get<IServerAddressesFeature>()
                .Addresses
                .Select(a => new Uri(a))
                .ToArray();
    }
}
