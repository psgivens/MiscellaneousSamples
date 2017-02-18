using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;

namespace psgcorelib.Threading
{
    public class MethodRequest
    {
        #region Fields
        private Delegate method;
        private object[] args;
        private Exception exception;
        private object result;
        private bool hasBeenExecuted;
        #endregion

        #region Initialize and Teardown
        public MethodRequest(Delegate method, object[] args)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.method = method;

            ResultHandle = new AsyncResult();
        }
        #endregion
        
        public void DynamicInvoke()
        {
            try
            {
                result = method.DynamicInvoke(args);
            }
            catch (Exception exception)
            {
                this.exception = exception;
            }
            finally
            {
                hasBeenExecuted = true;
            }
        }

        internal AsyncResult ResultHandle { get; private set; }

        public object Result
        {
            get{
                if (!hasBeenExecuted)
                    throw new InvalidOperationException("Cannot access Result until the method has been Invoked.");

                if (exception != null)
                    throw exception;

                return result;
            }
        }
    }

}
