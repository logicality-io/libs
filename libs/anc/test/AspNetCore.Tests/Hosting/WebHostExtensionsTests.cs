using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Shouldly;
using Xunit;

namespace Logicality.AspNetCore.Hosting
{
    public class WebHostExtensionsTests
    {
        [Fact]
        public async Task Can_get_server_port()
        {
            using var webHost = WebHost
                .CreateDefaultBuilder(Array.Empty<string>())
                .Configure(app => {})
                .UseKestrel()
                .UseUrls("http://127.0.0.1:0")
                .Build();

            await webHost.StartAsync();

            var serverPort = webHost.GetServerPort();

            serverPort.ShouldNotBe(0);
        }
    }
}
