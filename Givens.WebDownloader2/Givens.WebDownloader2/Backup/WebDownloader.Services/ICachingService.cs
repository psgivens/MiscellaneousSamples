using PhillipScottGivens.WebDownloader.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PhillipScottGivens.WebDownloader.Services
{
    [ServiceContract]
    public interface ICachingService
    {
        [OperationContract]
        int StartDownload(string address);

        [OperationContract]
        DownloadStatus GetDownloadStatus(int id);

        [OperationContract]
        string GetDownloadedHtml(int id);
    }
}
