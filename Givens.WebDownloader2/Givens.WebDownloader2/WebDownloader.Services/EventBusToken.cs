using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.WebDownloader.Services
{
    public enum EventBusToken
    {
        DownlaodRequested,
        DownloadedDelegated,
        DownloadBegin,
        DownloadComplete,
        DownloadErrored
    }
}
