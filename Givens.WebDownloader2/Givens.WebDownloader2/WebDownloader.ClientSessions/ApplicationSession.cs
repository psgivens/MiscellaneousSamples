using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDownloader.ClientSessions.Data;

namespace WebDownloader.ClientSessions
{
    public class ApplicationSession : INotifyPropertyChanged, IDisposable
    {
        #region Fields
        private ObservableCollection<DownloadRecord> objects
            = new ObservableCollection<DownloadRecord>();
        WebDownloaderClient requester = new WebDownloaderClient();
        #endregion

        public ApplicationSession()
        {
            Records.Add(new DownloadRecord
            {
                Address = "http://www.phillipscottgivens.com",
                Status = ClientDownloadStatus.NotSubmitted
            });

            Records.Add(new DownloadRecord
            {
                Address = "http://www.barakobama.com",
                Status = ClientDownloadStatus.NotSubmitted
            });

            Records.Add(new DownloadRecord
            {
                Address = "http://www.georgebush.com",
                Status = ClientDownloadStatus.NotSubmitted
            });

            Records.Add(new DownloadRecord
            {
                Address = "http://www.nonsensedotorg.com",
                Status = ClientDownloadStatus.NotSubmitted
            });
        }

        public void Dispose()
        {
            requester.Stop();
        }

        #region Properties
        public ObservableCollection<DownloadRecord> Records
        {
            get
            {
                return objects;
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Notifications
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public void AddUrls(string urlsAsString)
        {
            string[] urls = urlsAsString.Split(',');
            foreach (string url in urls)
                Records.Add(new DownloadRecord
                {
                    Id=0,
                    Address = url,
                    Status = ClientDownloadStatus.NotSubmitted
                });
        }

        public void StartDownloading()
        {
            foreach (var item in Records)
                requester.Process(item);
        }
    }
}
