using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastmastersRecords.Data;

namespace ToastmastersRecords.ViewModels {
    public class SchedulerViewModel : INotifyPropertyChanged, IDisposable {
        private readonly TIDbContext context;
        private MeetingViewModel _selectedSchedule;
        private readonly ObservableCollection<ClubMeeting> _clubMeetings;
        public ObservableCollection<ClubMeeting> ClubMeetings { get { return _clubMeetings; } }
        public MeetingViewModel SelectedSchedule {
            get { return _selectedSchedule; }
            set {
                _selectedSchedule = value;
                Notify("SelectedSchedule");
            }
        }

        public SchedulerViewModel(TIDbContext context) {
            this.context = context;
            _clubMeetings = new ObservableCollection<ClubMeeting>(context.ClubMeetings);
        }

        private void Notify(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void Dispose() {
            context.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
