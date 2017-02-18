using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parakeet.DomainServices
{
    public class AsyncResult<TValue>
    {
        #region Initialize and Teardown
        public AsyncResult(TValue value)
        {
            this.value = value;
        }

        public AsyncResult(Exception exception)
        {
            Exception = exception;
        }
        #endregion

        private TValue value;
        public TValue Value
        {
            get
            {
                if (Exception != null)
                    throw Exception;

                return value;
            }
        }

        public Exception Exception { get; private set; }
    }
}
