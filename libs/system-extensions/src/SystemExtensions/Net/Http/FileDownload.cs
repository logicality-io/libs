using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Logicality.SystemExtensions.Net.Http
{
    public static class FileDownload
    {
        public static async Task DownloadFile(
            this HttpMessageInvoker httpMessageInvoker,
            Uri source,
            string filePath,
            bool overwrite = true,
            CancellationToken cancellationToken = default)
        {
            if (File.Exists(filePath) && !overwrite)
            {
                return;
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, source);
            var responseMessage = await httpMessageInvoker.SendAsync(request, cancellationToken);
            responseMessage.EnsureSuccessStatusCode();
            await using var contentStream = await responseMessage.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await contentStream.CopyToAsync(fileStream, cancellationToken);
        }
    }
}
