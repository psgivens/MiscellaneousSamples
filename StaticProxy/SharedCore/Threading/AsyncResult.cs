using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace psgcorelib.Threading
{
    public class AsyncResult : DisposableObject, IAsyncResult
    {
        private ManualResetEvent waitHandle = new ManualResetEvent(false);

        protected override void Dispose(bool isDisposing)
        {
            waitHandle.Dispose();
            base.Dispose(isDisposing);
        }

        public object AsyncState
        {
            get { throw new NotSupportedException (); }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { return waitHandle; }
        }

        public bool CompletedSynchronously
        {
            get;
            internal set;
        }

        public bool IsCompleted
        {
            get;
            internal set; 
        }
    }
}
