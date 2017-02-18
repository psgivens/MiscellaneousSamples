using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parakeet.DomainServices
{
    public delegate void AsyncCallback<TValue>(AsyncResult<TValue> result);
}
