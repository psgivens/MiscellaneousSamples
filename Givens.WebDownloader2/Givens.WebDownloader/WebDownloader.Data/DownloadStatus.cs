using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.WebDownloader.Data
{
    public enum DownloadStatus
    {
        Requested,
        Downloading,
        Downloaded,
        Errored
    }
}
