using PhillipScottGivens.WebDownloader.Data;
using System.Linq;

namespace PhillipScottGivens.WebDownloader.Services
{
    public class DownloadItemRepository : IDownloadItemRepository
    {
        //private static object lockObject = new object();

        private readonly WebDownloaderContext context;

        public DownloadItemRepository()
        {
            context = new WebDownloaderContext();
        }

        public void Add(WebDownloader.Data.DownloadItem item)
        {
            context.Downloads.Add(item);
        }

        public DownloadItem Get(int id)
        {
            var items = context.Downloads.ToArray();
            return context.Downloads.Find(id);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}