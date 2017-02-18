using PhillipScottGivens.WebDownloader.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using System.Threading;
using PhillipScottGivens.WebDownloader.Services;
using PhillipScottGivens.Common;

namespace PhillipScottGivens.ConsoleApplication
{
    class Program
    {
        const string Address = "http://www.phillipscottgivens.com";

        static void Main(string[] args)
        {
            // Glue together the system.
            ContainerBuilder builder = new ContainerBuilder();

            // Singletons
            builder.RegisterType<ConsoleLogger>().As<ILogger>().SingleInstance();
            builder.RegisterType<EventBus>().As<IEventBus>().SingleInstance();
            builder.RegisterType<ThreadPoolQueue>().As<IProcessingQueue>().SingleInstance();

            // Per request.
            builder.RegisterType<CachingService>().As<ICachingService>();
            builder.RegisterType<DownloaderClient>().As<IDownloaderClient>();
            builder.RegisterType<DownloadItemRepository>().As<IDownloadItemRepository>();

            // Get the relevant parts. 
            IContainer container = builder.Build();
            ILogger logger = container.Resolve<ILogger>();
            ICachingService cachingService = container.Resolve<ICachingService>();
            IEventBus eventBus = container.Resolve<IEventBus>();
            Func<IDownloadItemRepository> createDownloadRepository = container.Resolve<Func<IDownloadItemRepository>>();

            var mainThreadReady = new AutoResetEvent(false);
            var downloadBegin = new ManualResetEvent(false);
            var downloadComplete = new ManualResetEvent(false);
            eventBus.Listen(EventBusToken.DownloadBegin, sender =>
                {
                    // This code runs from the service thread. It is 
                    // meant to synchronize the threads. 

                    // Wait for the main thread to be ready to respond
                    // to the download beginning. 
                    mainThreadReady.WaitOne();

                    // Alert the main thread that we have reached this point.
                    downloadBegin.Set();

                    // Wait until the main thread has finished testin whatever
                    // it wanted to test. 
                    mainThreadReady.WaitOne();
                });
            eventBus.Listen(EventBusToken.DownloadComplete, sender => downloadComplete.Set());

            // Start the download
            var id = cachingService.StartDownload(Address);

            // Let the processing thread know that we have unwound from StartDownload
            mainThreadReady.Set();

            // Wait for the processing thread to hit the DownloadBegin state. 
            downloadBegin.WaitOne();

            // Assert that the download item is in a Begin state. 
            if (cachingService.GetDownloadStatus(id) != DownloadStatus.Downloading)
                throw new Exception("item is incorrect state");

            // Let the processing thread know that we have completed our inspection. 
            mainThreadReady.Set();

            // Wait for the download to complete. 
            downloadComplete.WaitOne();

            // Assert that the download item is in a Complete state. 
            if (cachingService.GetDownloadStatus(id) != DownloadStatus.Downloaded)
                throw new Exception("item is incorrect state");
            
            // Report some output. 
            logger.WriteLine("logged value {0}", id);
            logger.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
