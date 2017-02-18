using PhillipScottGivens.WebDownloader.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhillipScottGivens.WebDownloader.Services
{
    public interface IDownloadItemRepository : IDisposable
    {
        void Add(DownloadItem item);
        DownloadItem Get(int id);
        void SaveChanges();
    }
}