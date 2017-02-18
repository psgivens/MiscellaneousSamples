using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.WebDownloader.Data
{
    public class WebDownloaderContext : DbContext
    {
        public DbSet<DownloadItem> Downloads { get; set; }
    }
}
