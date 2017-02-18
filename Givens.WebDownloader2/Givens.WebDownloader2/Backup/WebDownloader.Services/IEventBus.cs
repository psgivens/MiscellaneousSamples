using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.WebDownloader.Services
{
    public interface IEventBus
    {
        void Notify(object sender, EventBusToken token);
        void Listen(EventBusToken token, Action<object> action);
    }
}
