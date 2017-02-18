using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Parakeet.Infrastructure
{
    public class DispatcherInvoker : System.ComponentModel.ISynchronizeInvoke
    {
        private readonly Dispatcher dispatcher;
        public DispatcherInvoker(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            dispatcher.BeginInvoke(method, args);
            return null;
        }

        public object EndInvoke(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public object Invoke(Delegate method, object[] args)
        {
            throw new NotImplementedException();
        }

        public bool InvokeRequired
        {
            get { throw new NotImplementedException(); }
        }
    }
}
