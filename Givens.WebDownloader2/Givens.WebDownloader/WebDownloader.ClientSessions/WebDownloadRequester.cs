using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebDownloader.ClientSessions.CachingServiceReference;
using WebDownloader.ClientSessions.Data;

namespace WebDownloader.ClientSessions
{
    public class WebDownloaderClient
    {
        private readonly object syncObject = new object();
        private readonly Queue records = Queue.Synchronized(new Queue());
        private readonly ManualResetEvent resetEvent = new ManualResetEvent(false);
        private volatile bool isRunning = true;
        private readonly SynchronizationContext callingContext;

        public WebDownloaderClient()
        {
            callingContext = SynchronizationContext.Current;

            var thread = new Thread(Run);
            //thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
        }

        private void Run()
        {
            bool isRunning;
            lock (syncObject)
                isRunning = this.isRunning;

            while (isRunning)
            {
                if (records.Count == 0)
                    resetEvent.WaitOne();

                resetEvent.Reset();

                var record = records.Dequeue() as DownloadRecord;
                if (record == null)
                    continue;

                var serviceClient = new CachingServiceClient();

                switch (record.Status)
                {
                    case ClientDownloadStatus.NotSubmitted:
                        int id = serviceClient.StartDownload(record.Address);
                        callingContext.Post(state =>
                            {
                                record.Id = (int)state;
                                record.Status = ClientDownloadStatus.Submitted;
                            }, id);
                        records.Enqueue(record);
                        resetEvent.Set();
                        break;

                    case ClientDownloadStatus.Submitted:
                    case ClientDownloadStatus.Downloading:

                        System.Threading.Thread.Sleep(200);

                        DownloadStatus status = serviceClient.GetDownloadStatus(record.Id);
                        callingContext.Post(state =>
                        {
                            var s = (DownloadStatus)state;
                            record.Status
                                = s == DownloadStatus.Downloading ? ClientDownloadStatus.Downloading
                                : s == DownloadStatus.Downloaded ? ClientDownloadStatus.Downloaded
                                : s == DownloadStatus.Errored ? ClientDownloadStatus.Error
                                : ClientDownloadStatus.Submitted;
                        }, status);

                        // Throw it back int the queue for further polling. 
                        records.Enqueue(record);
                        resetEvent.Set();
                        break;

                    case ClientDownloadStatus.Downloaded:
                    case ClientDownloadStatus.Error:
                    default:
                        break;
                }

                lock (syncObject)
                    isRunning = this.isRunning;
            }
        }

        public void Process(DownloadRecord record)
        {
            records.Enqueue(record);
            resetEvent.Set();
        }

        public void Stop()
        {
            lock (syncObject)
                isRunning = false;

            resetEvent.Set();
        }
    }
}
