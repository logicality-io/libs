using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Hosting;

public static class WebApplicationExtensions
{
    public static Uri[] GetServerUris(this WebApplication webApplication)
    {
        var host                   = webApplication.Services.GetRequiredService<IHost>();
        var serverAddressesFeature = host.Services.GetRequiredService<IServerAddressesFeature>();
        return serverAddressesFeature.Addresses.Select(a => new Uri(a)).ToArray();
    }
}