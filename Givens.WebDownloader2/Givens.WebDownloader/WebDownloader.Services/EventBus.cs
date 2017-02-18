using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.WebDownloader.Services
{
    public class EventBus : IEventBus
    {
        Dictionary<EventBusToken, Action<object>> actions
            = new Dictionary<EventBusToken, Action<object>>();

        public void Notify(object sender, EventBusToken token)
        {
            Action<object> action = null;
            if (actions.TryGetValue(token, out action))
                action(sender);
        }

        public void Listen(EventBusToken token, Action<object> action)
        {
            Action<object> existingAction = null;
            if (actions.TryGetValue(token, out existingAction))
            {
                actions[token] = (Action<object>)Delegate.Combine(existingAction, action);
            }
            else
            {
                actions.Add(token, action);
            }
        }
    }
}
