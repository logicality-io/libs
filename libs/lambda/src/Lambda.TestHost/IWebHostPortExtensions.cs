using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Logicality.AWS.Lambda.TestHost
{
    internal static class WebHostPortExtensions
    {
        internal static IEnumerable<Uri> GetUris(this IWebHost host) =>
            host
                .ServerFeatures
                .Get<IServerAddressesFeature>()
                .Addresses
                .Select(a => new Uri(a));
    }
}