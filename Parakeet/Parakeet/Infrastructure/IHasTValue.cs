using System;
using System.Collections.Generic;
using System.Linq;

namespace Parakeet.Infrastructure
{
    public interface IHasTValue<TValue>
    {
        TValue Value { get; }
    }
}
