using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Parakeet.Sessions;
using Parakeet.Infrastructure;

namespace Parakeet.Presenters
{
    public class ThoughtEntryPresenter :Presenter<SubjectSession>
    {
        //private readonly SubjectSession subjectSession;
        public ThoughtEntryPresenter(SubjectSession manager)
            : base(manager)
        {
            EnterMessage = new DelegateCommand(manager.PostThought);
            manager.PropertyChanged += manager_PropertyChanged;
        }

        private void manager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "PendingThought":
                    RaisePropertyChanged("Message");
                    break;
                default:
                    break;
            }
        }
        public string Message
        {
            get
            {
                return Value.PendingThought;
            }
            set
            {
                Value.PendingThought = value;
            }
        }
        public ICommand EnterMessage { get; private set; }
    }
}
