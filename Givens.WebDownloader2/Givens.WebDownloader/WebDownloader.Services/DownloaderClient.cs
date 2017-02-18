using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace PhillipScottGivens.WebDownloader.Services
{
    public class DownloaderClient : IDownloaderClient
    {
        private readonly WebClient webClient = new WebClient();
        public string DownloadString(string address)
        {
            return webClient.DownloadString(address);
        }

        public void Dispose()
        {
            webClient.Dispose();
        }
    }
}