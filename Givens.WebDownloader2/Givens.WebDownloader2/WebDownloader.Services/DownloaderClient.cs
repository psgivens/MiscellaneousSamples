using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace PhillipScottGivens.WebDownloader.Services
{
    public class DownloaderClient : IDownloaderClient
    {
        private readonly WebClient webClient = new WebClient();
        public Task<string> DownloadStringAsync(string address)
        {
            return webClient.DownloadStringTaskAsync(address);
        }

        public void Dispose()
        {
            webClient.Dispose();
        }
    }
}