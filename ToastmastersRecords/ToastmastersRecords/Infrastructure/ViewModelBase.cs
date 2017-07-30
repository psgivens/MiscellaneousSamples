using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToastmastersRecords.Data;

namespace ToastmastersRecords.Infrastructure {
    public class ViewModelBase : INotifyPropertyChanged, IDisposable {
        protected TIDbContext Context { get; private set; }
        public ViewModelBase(TIDbContext context) {
            this.Context = context;
        }
        protected void Notify(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void Dispose() {
            Context.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
