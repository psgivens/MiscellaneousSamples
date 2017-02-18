using PhillipScottGivens.WebDownloader.Data;
using PhillipScottGivens.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.WebDownloader.Services
{
    public class DownloadService : IDownloadService
    {
        #region Dependencies
        private readonly Func<IDownloadItemRepository> createRepository;
        private readonly IEventBus eventBus;
        private readonly ILogger logger;
        private readonly Func<IDownloaderClient> createClient;
        #endregion

        public DownloadService(
            IEventBus eventBus, 
            ILogger logger, 
            Func<IDownloadItemRepository> createRepository,
            Func<IDownloaderClient> createClient)
        {
            this.eventBus = eventBus.Guard();
            this.createRepository = createRepository.Guard();
            this.logger = logger.Guard();
            this.createClient = createClient.Guard();
        }

        public void Download(int id)
        {
            string address;
            using (IDownloadItemRepository repository = createRepository())
            {
                var item = repository.Get(id);
                item.Status = DownloadStatus.Downloading;
                address = item.Address;
                repository.SaveChanges();
            }

            eventBus.Notify(this, EventBusToken.DownloadBegin);

            string value;
            if (DownloadFile(address, out value))
            {
                using (IDownloadItemRepository repository = createRepository())
                {
                    var item = repository.Get(id);
                    item.Html = value;
                    item.Status = DownloadStatus.Downloaded;
                    repository.SaveChanges();
                }

                eventBus.Notify(this, EventBusToken.DownloadComplete);
            }
            else
            {
                using (IDownloadItemRepository repository = createRepository())
                {
                    var item = repository.Get(id);
                    item.Status = DownloadStatus.Errored;
                    repository.SaveChanges();
                }
                eventBus.Notify(this, EventBusToken.DownloadErrored);
            }
        }

        #region Utility Methods

        /// <remarks>
        /// Please exuse the signature and exception handling of this 
        /// method. This is not my normal style. 
        /// </remarks>
        private bool DownloadFile(string address, out string html)
        {
            html = string.Empty;
            logger.WriteLine("Downloading address: " + address);
            try
            {
                using (IDownloaderClient downloaderClient = createClient())
                {
                    html = downloaderClient.DownloadString(address);
                }
                logger.WriteLine("Chars read: " + html.Length);
                logger.WriteLine("Beginning: " + html.Substring(0, 200));
                return true;
            }
            catch (System.Net.WebException webException)
            {
                logger.WriteLine("Something went wrong with the download: " + webException.Message);
                return false;
            }
        }
        #endregion
    }
}
