using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.WebDownloader.Services
{
    public interface IDownloaderClient : IDisposable
    {
        Task<string> DownloadStringAsync(string address);
    }
}
