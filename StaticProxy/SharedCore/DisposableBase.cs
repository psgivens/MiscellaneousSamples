using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace psgcorelib
{
    public class DisposableObject : IDisposable
    {
        #region Fields
        private bool isDisposed;
        #endregion

        #region Initialize and Teardown
        public void Dispose()
        {
            if (isDisposed)
                return;

            try
            {

                Dispose(true);
            }
            finally
            {
                isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~DisposableObject()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool isDisposing)
        {
        }
        #endregion
    }
}
