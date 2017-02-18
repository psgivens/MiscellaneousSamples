using PhillipScottGivens.WebDownloader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.WebDownloader.Services.Tests
{
    class FailureDownloaderClient : IDownloaderClient
    {
        public async Task<string> DownloadStringAsync(string address)
        {
            // Try to simulate a network connection
            System.Threading.Thread.Sleep(200);

            throw new WebException("Some web exception from trying to receive this url.");
        }

        public void Dispose()
        {
        }
    }
}
