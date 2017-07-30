using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Technewlogic.WpfDialogManagement;
using ToastmastersRecords.Data;
using ToastmastersRecords.Infrastructure;

namespace ToastmastersRecords.ViewModels {
    public class ApplicationViewModel : ViewModelBase {              
        private readonly DialogManager _dialogManager;
        private DialogManager dialogManager;

        internal ApplicationViewModel(DialogManager dialogManager) : base(new TIDbContext()) {
            _membersViewModel = new MembersViewModel(Context);
            _schedulerViewModel = new SchedulerViewModel(Context);
            _dialogManager = dialogManager;
            NewMemberMessage = new ActionCommand(ShowNewMemberMessageDialog);
        }
        
        public static ApplicationViewModel Instance { get; internal set; }

        private readonly MembersViewModel _membersViewModel;
        public MembersViewModel MembersViewModel { get { return _membersViewModel; } }

        private readonly SchedulerViewModel _schedulerViewModel;
        public SchedulerViewModel SchedulerViewModel { get { return _schedulerViewModel; } }

        public ICommand NewMemberMessage { get; private set; }
        private void ShowNewMemberMessageDialog() {
            var control = new Controls.MemberMessageControl();
            var viewModel = new MemberMessageViewModel(Context, MembersViewModel.Member);
            control.DataContext = viewModel;
            var dialog = _dialogManager.CreateCustomContentDialog(control, "Member Message", DialogMode.Ok);
            dialog.DialogClosed += (sender, e) => {
                // TODO: Refresh messages, day off requests, and role requests througout the application
            };
            dialog.Show();
        }
        
        public void Upsert(BaseEntity entity) {
            Context.Upsert(entity);
            Context.SaveChanges();
        }
    }
}
