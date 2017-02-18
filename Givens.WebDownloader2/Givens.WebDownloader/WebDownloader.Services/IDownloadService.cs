using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.WebDownloader.Services
{
    [ServiceContract]
    public interface IDownloadService
    {
        [OperationContract(IsOneWay = true)]
        void Download(int id);
    }
}
