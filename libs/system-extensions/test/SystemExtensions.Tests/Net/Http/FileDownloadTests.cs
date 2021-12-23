using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Logicality.SystemExtensions.Net.Http
{
    public class DownloadFileTests
    {
        [Fact]
        public async Task Can_download_file()
        {
            var client = new HttpClient();

            var sourceUri = new Uri("https://google.com/robots.txt");
            var destinationPath = $"{Path.GetTempPath()}{Guid.NewGuid()}.txt";
            await client.DownloadFile(sourceUri, destinationPath);

            File.Exists(destinationPath).ShouldBeTrue();
        }
    }
}
