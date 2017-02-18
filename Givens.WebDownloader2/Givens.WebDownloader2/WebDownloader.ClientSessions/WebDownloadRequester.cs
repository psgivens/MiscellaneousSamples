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
        private readonly Queue records = Queue.Synchronized(new Queue());
        private readonly ManualResetEvent resetEvent = new ManualResetEvent(false);
        private bool isRunning = true;
        private readonly SynchronizationContext synchronizationContext;

        public WebDownloaderClient()
        {
            synchronizationContext = SynchronizationContext.Current;

            var thread = new Thread(Run);            
            thread.Start();
        }

        private void Run()
        {
            while (Volatile.Read(ref this.isRunning))
            {
                if (records.Count == 0)
                    resetEvent.WaitOne();

                if (!Volatile.Read(ref this.isRunning))
                    break;

                resetEvent.Reset();

                var record = records.Dequeue() as DownloadRecord;
                var serviceClient = new CachingServiceClient();

                switch (record.Status)
                {
                    case ClientDownloadStatus.NotSubmitted:
                        record.Id = serviceClient.StartDownload(record.Address);
                        UpdateState(record, ClientDownloadStatus.Submitted);
                        records.Enqueue(record);
                        break;

                    case ClientDownloadStatus.Submitted:
                    case ClientDownloadStatus.Delegated:
                    case ClientDownloadStatus.Downloading:

                        System.Threading.Thread.Sleep(200);

                        DownloadStatus status = serviceClient.GetDownloadStatus(record.Id);
                        UpdateState(record,
                              status == DownloadStatus.Delegated ? ClientDownloadStatus.Delegated
                            : status == DownloadStatus.Downloading ? ClientDownloadStatus.Downloading
                            : status == DownloadStatus.Downloaded ? ClientDownloadStatus.Downloaded
                            : status == DownloadStatus.Errored ? ClientDownloadStatus.Error
                            : ClientDownloadStatus.Submitted);

                        // Throw it back into the queue for further polling. 
                        records.Enqueue(record);

                        break;

                    case ClientDownloadStatus.Downloaded:
                    case ClientDownloadStatus.Error:
                    default:
                        break;
                }
            }
        }

        private void UpdateState(DownloadRecord record, ClientDownloadStatus status)
        {
            synchronizationContext.Post(state => record.Status = (ClientDownloadStatus)state, status);
        }

        public void Process(DownloadRecord record)
        {
            records.Enqueue(record);
            resetEvent.Set();
        }

        public void Stop()
        {
            Volatile.Write(ref isRunning, false);

            resetEvent.Set();
        }
    }
}
