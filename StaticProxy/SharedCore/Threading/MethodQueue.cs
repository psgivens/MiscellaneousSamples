using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;

namespace psgcorelib.Threading
{
    // TODO: Refactor to use the Monitor class to lock.
    public class MethodQueue : DisposableObject, ISynchronizeInvoke
    {
        #region Fields
        private Queue<MethodRequest> methodQueue = new Queue<MethodRequest>();
        private Thread thread;
        private ManualResetEvent methodInQueueHandle = new ManualResetEvent(false);
        private ManualResetEvent finishHandle = new ManualResetEvent(false);
        private bool isRunning = true;
        #endregion

        #region Initialize and Teardown
        public MethodQueue(string threadName)
        {
            thread = new Thread(MethodPump);
            thread.Name = threadName;

            ManualResetEvent startSignal = new ManualResetEvent(false);
            thread.Start(startSignal);
            startSignal.WaitOne(-1);
        }

        protected override void Dispose(bool isDisposing)
        {
            Shutdown(true);
            base.Dispose(isDisposing);
        }
        #endregion

        #region Message Pump
        private void MethodPump(object startSignal)
        {
            ((ManualResetEvent)startSignal).Set();

            MethodRequest methodRequest;

            while (isRunning)
            {
                lock (methodQueue)
                    methodRequest = methodQueue.Dequeue();

                if (methodRequest != null)
                    methodRequest.DynamicInvoke();

                lock (methodQueue)
                    if (isRunning && methodQueue.Count == 0)
                        methodInQueueHandle.Reset();

                methodInQueueHandle.WaitOne(-1);
            }

            finishHandle.Set();
        }
        #endregion

        #region ISynchronizedInvoke
        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            var request = new MethodRequest(method, args);
            lock (methodQueue)
                methodQueue.Enqueue(request);
            return request.ResultHandle;
        }

        public object EndInvoke(IAsyncResult asyncHandle)
        {
            using (asyncHandle as AsyncResult)
            {
                asyncHandle.AsyncWaitHandle.WaitOne(-1);

                object result = asyncHandle.AsyncState;
                var exception = result as Exception;
                if (exception != null)
                    throw exception;

                return result;
            }
        }

        public object Invoke(Delegate method, object[] args)
        {
            var asyncHandle = BeginInvoke(method, args) as AsyncResult;
            object result = EndInvoke(asyncHandle);
            asyncHandle.CompletedSynchronously = true;
            return result;
        }

        public bool InvokeRequired
        {
            get { return Thread.CurrentThread.ManagedThreadId != thread.ManagedThreadId; }
        }
        #endregion

        public void Shutdown(bool forcefully)
        {
            isRunning = false;
            lock (methodQueue)
                methodInQueueHandle.Set();

            if (forcefully)
                thread.Abort();
            else
                thread.Join(TimeSpan.FromSeconds(5));

            finishHandle.WaitOne(-1);
        }
    }
}
