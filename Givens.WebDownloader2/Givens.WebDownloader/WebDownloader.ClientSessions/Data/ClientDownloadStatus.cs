using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDownloader.ClientSessions.Data
{
    public enum ClientDownloadStatus
    {
        NotSubmitted,
        Submitted,
        Downloading,
        Downloaded,
        Error
    }
}
