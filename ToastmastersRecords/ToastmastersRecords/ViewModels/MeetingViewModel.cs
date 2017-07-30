using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastmastersRecords.Data;

namespace ToastmastersRecords {
    public class MeetingViewModel : INotifyPropertyChanged, IDisposable {
        private readonly TIDbContext context;

        private DateTime _selectedDate;
        public DateTime SelectedDate {
            get { return _selectedDate; }
            set {
                _selectedDate = value;
                Notify("SelectedDate");
            }
        }

        private IEnumerable<RoleAssignment> _assignments;
        public IEnumerable<RoleAssignment> Assignments {
            get { return _assignments; }
            set {
                _assignments = value;
                Notify("Assignments");
            }
        }
        private string _theme;
        public string Theme {
            get { return _theme; }
            set { _theme = value; Notify("Theme"); }
        }

        private Member _toastmaster;
        public Member Toastmaster {
            get { return _toastmaster; }
            set {
                _toastmaster = value;
                Notify("Toastmaster ");
            }
        }
        private Member _tableTopicsMaster;
        public Member TableTopicsMaster {
            get { return _tableTopicsMaster; }
            set {
                _tableTopicsMaster = value;
                Notify("TableTopicsMaster ");
            }
        }

        private Member _generalEvaluator;
        public Member GeneralEvaluator {
            get { return _generalEvaluator; }
            set {
                _generalEvaluator = value;
                Notify("GeneralEvaluator ");
            }
        }

        private Member _jokeMaster;
        public Member JokeMaster {
            get { return _jokeMaster; }
            set {
                _jokeMaster = value;
                Notify("JokeMaster ");
            }
        }

        private Member _openingThought;
        public Member OpeningThought {
            get { return _openingThought; }
            set {
                _openingThought = value;
                Notify("OpeningThought ");
            }
        }

        private Member _closingThought;
        public Member ClosingThought {
            get { return _closingThought; }
            set {
                _closingThought = value;
                Notify("ClosingThought ");
            }
        }

        private Member _grammarian;
        public Member Grammarian {
            get { return _grammarian; }
            set {
                _grammarian = value;
                Notify("Grammarian ");
            }
        }

        private Member _fillerCounter;
        public Member FillerCounter {
            get { return _fillerCounter; }
            set {
                _fillerCounter = value;
                Notify("FillerCounter ");
            }
        }

        private Member _timer;
        public Member Timer {
            get { return _timer; }
            set {
                _timer = value;
                Notify("Timer ");
            }
        }

        private Member _videographer;
        public Member Videographer {
            get { return _videographer; }
            set {
                _videographer = value;
                Notify("Videographer ");
            }
        }

        public MeetingViewModel(TIDbContext context) {
            this.context = context;
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
