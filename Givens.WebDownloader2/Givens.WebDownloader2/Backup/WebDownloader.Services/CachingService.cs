using PhillipScottGivens.Common;
using PhillipScottGivens.WebDownloader.Data;
using System;
using System.ServiceModel;

namespace PhillipScottGivens.WebDownloader.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class CachingService : ICachingService
    {
        #region Dependencies
        private readonly ILogger logger;
        private readonly IEventBus eventBus;
        private readonly Func<IDownloaderClient> createClient;
        private readonly Func<IDownloadItemRepository> createRepository;
        #endregion

        #region Constructor
        public CachingService(
            ILogger logger,
            IEventBus eventBus,
            Func<IDownloaderClient> createClient,
            Func<IDownloadItemRepository> createRepository)
        {
            this.logger = logger.Guard();
            this.eventBus = eventBus.Guard();
            this.createClient = createClient.Guard();
            this.createRepository = createRepository.Guard();
        }
        #endregion

        public int StartDownload(string address)
        {
            // put a record in the database.
            int id;

            using (IDownloadItemRepository repository = createRepository())
            {
                var item = new DownloadItem()
                {
                    Address = address,
                    Status = DownloadStatus.Requested
                };
                repository.Add(item);
                repository.SaveChanges();

                // get the database generated id. 
                id = item.Id;
            }

            eventBus.Notify(this, EventBusToken.DownlaodRequested);

            var channelFactory = new ChannelFactory<IDownloadService>("DLS");
            var downloadService = channelFactory.CreateChannel();
            using (downloadService as IDisposable)
                downloadService.Download(id);

            return id;
        }

        public DownloadStatus GetDownloadStatus(int id)
        {
            using (var repository = createRepository())
            {
                DownloadItem item = repository.Get(id);
                return item.Status;
            }
        }

        public string GetDownloadedHtml(int id)
        {
            using (var repository = createRepository())
            {
                DownloadItem item = repository.Get(id);
                return item.Html;
            }
        }
    }
}
