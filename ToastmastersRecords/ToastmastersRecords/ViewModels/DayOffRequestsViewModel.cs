using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastmastersRecords.Data;
using ToastmastersRecords.Infrastructure;

namespace ToastmastersRecords.ViewModels {
    public class DayOffRequestsViewModel : ViewModelBase {
        private MemberMessage _message;
        private Member _member;
        public DayOffRequestsViewModel(TIDbContext context, MemberMessage message) : base(context) {
            _message = message;

            DayOffRequests = (
                from req in context.DayOffRequests
                where req.Message.Id == message.Id
                select req).ToList();
        }
        public DayOffRequestsViewModel(TIDbContext context, Member member) : base(context) {
            _member = member;

            DayOffRequests = (
                from req in context.DayOffRequests
                where req.Message.MemberId == _member.Id
                select req).ToList();
        }

        private IList<DayOffRequest> _dayOffRequests;
        public IList<DayOffRequest> DayOffRequests {
            get { return _dayOffRequests; }
            set {
                _dayOffRequests = value;
                Notify("DayOffRequests");
            }
        }

        public DayOffRequest CreateDayOffRequest() {
            MemberMessage message;
            if (_message == null) {
                message = Context.MemberMessages.Create();
                message.Member = _member;
                message.DateEntered = DateTime.Now;
            }
            else { message = _message; }

            var req = Context.DayOffRequests.Create();
            req.Message = message;
            req.DateRequested = DateTime.Now;
            return req;
        }
    }
}
