using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.WebDownloader.Services
{
    public interface IDownloaderClient : IDisposable
    {
        string DownloadString(string address);
    }
}
