using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperEmbeddedWebServer
{
    public abstract class DataPacket
    {
        protected DataPacket(string type)
        {
            this.Type = type;
        }
        public string Type { get; private set; }
    }
}
