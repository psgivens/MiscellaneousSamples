using PhillipScottGivens.WebDownloader.Data;
using PhillipScottGivens.WebDownloader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.WebDownloader.Services.Tests
{
    public class FakeDownloadItemRepository : IDownloadItemRepository
    {
        private readonly List<DownloadItem> items
            = new List<DownloadItem>();
        private int lastId = int.MaxValue;

        public void Add(DownloadItem item)
        {
            items.Add(item);
        }

        public DownloadItem Get(int id)
        {
            return items.FirstOrDefault(item => item.Id == id);
        }

        public void SaveChanges()
        {
            foreach (var item in items)
                if (item.Id == 0)
                    item.Id = lastId--;
        }

        public void Dispose()
        {
        }
    }
}
