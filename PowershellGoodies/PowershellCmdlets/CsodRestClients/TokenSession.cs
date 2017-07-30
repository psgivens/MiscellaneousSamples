using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsodRestClients
{
    public class TokenSession
    {
        public string Token { get; set; }
        public string Secret { get; set; }
        public string Alias { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
