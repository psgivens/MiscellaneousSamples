using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhillipScottGivens.Common;
using PhillipScottGivens.WebDownloader.Data;
using System.Threading;

namespace PhillipScottGivens.WebDownloader.Services.Tests
{
    [TestClass]
    public class DownloaderServiceTests
    {
        const string Address = "http://www.phillipscottgivens.com";

        private ContainerBuilder InitializeBuilder()
        {
            // Glue together the system.
            ContainerBuilder builder = new ContainerBuilder();

            // Singletons
            builder.RegisterType<NullLogger>().As<ILogger>().SingleInstance();
            builder.RegisterType<EventBus>().As<IEventBus>().SingleInstance();

            // Per request.
            builder.RegisterType<CachingService>().As<ICachingService>();
            builder.RegisterType<FakeDownloadItemRepository>().As<IDownloadItemRepository>().SingleInstance();

            return builder;
        }

        [TestMethod]
        public void SucceedDownloadWithoutWcf()
        {
            #region Build the container
            ContainerBuilder builder = InitializeBuilder();

            // This download is going to succeed. 
            builder.RegisterType<SuccessDownloaderClient>().As<IDownloaderClient>();
            IContainer container = builder.Build();
            #endregion

            #region Get some utility classes
            // Get the relevant parts.             
            ICachingService cachingService = container.Resolve<ICachingService>();
            IEventBus eventBus = container.Resolve<IEventBus>();
            #endregion

            #region Create some synchronization mechanisms using the EventBus
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
            #endregion

            #region Check that we moved into a Begin state as anticipated
            // Start the download
            var id = cachingService.StartDownload(Address);

            // Let the processing thread know that we have unwound from StartDownload
            mainThreadReady.Set();

            // Wait for the processing thread to hit the DownloadBegin state. 
            downloadBegin.WaitOne();

            // Assert that the download item is in a Begin state. 
            Assert.IsTrue(cachingService.GetDownloadStatus(id) == DownloadStatus.Downloading);

            // Let the processing thread know that we have completed our inspection. 
            mainThreadReady.Set();
            #endregion

            #region Check that we downloaded the file.
            // Wait for the download to complete. 
            downloadComplete.WaitOne();

            // Assert that the download item is in a Complete state. 
            Assert.IsTrue(cachingService.GetDownloadStatus(id) == DownloadStatus.Downloaded);
            #endregion
        }

        [TestMethod]
        public void FailDownloadWithoutWcf()
        {
            #region Build the container
            // Glue together the system.
            ContainerBuilder builder = InitializeBuilder();

            // This download is going to fail.
            builder.RegisterType<FailureDownloaderClient>().As<IDownloaderClient>();
            IContainer container = builder.Build();
            #endregion

            #region Get some utility classes
            // Get the relevant parts. 
            ICachingService cachingService = container.Resolve<ICachingService>();
            IEventBus eventBus = container.Resolve<IEventBus>();
            #endregion

            #region Create some synchronization mechanisms using the EventBus
            var mainThreadReady = new AutoResetEvent(false);
            var downloadBegin = new ManualResetEvent(false);
            var downloadErrored = new ManualResetEvent(false);
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
            eventBus.Listen(EventBusToken.DownloadErrored, sender => downloadErrored.Set());
            #endregion

            #region Check that we moved into a Begin state as anticipated
            // Start the download
            var id = cachingService.StartDownload(Address);

            // Let the processing thread know that we have unwound from StartDownload
            mainThreadReady.Set();

            // Wait for the processing thread to hit the DownloadBegin state. 
            downloadBegin.WaitOne();

            // Assert that the download item is in a Begin state. 
            Assert.IsTrue(cachingService.GetDownloadStatus(id) == DownloadStatus.Downloading);

            // Let the processing thread know that we have completed our inspection. 
            mainThreadReady.Set();
            #endregion

            #region Check that we downloaded the file.
            // Wait for the download to complete. 
            downloadErrored.WaitOne();

            // Assert that the download item is in a Complete state. 
            Assert.IsTrue(cachingService.GetDownloadStatus(id) == DownloadStatus.Errored);
            #endregion
        }
    }
}
