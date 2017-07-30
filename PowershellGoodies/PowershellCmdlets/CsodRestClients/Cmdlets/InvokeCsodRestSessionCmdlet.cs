using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace CsodRestClients.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Invoke, "CsodRestSession")]
    [OutputType(typeof(TokenSession))]
    public class InvokeCsodRestSessionCmdlet : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string ApiKey { get; set; }

        [Parameter(Mandatory = true)]
        public string ApiSecret { get; set; }

        [Parameter(Mandatory = true)]
        public string Domain { get; set; }

        [Parameter(Mandatory = true)]
        public string UserName { get; set; }

        [Parameter(Mandatory = false)]
        public string Alias { get; set; }

        private const string _uri = "services/api/STS/Session?userName={0}&alias={1}";
        protected override void ProcessRecord()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Alias))
                {
                    Alias = Guid.NewGuid().ToString();
                }
                string uri = string.Format(_uri, UserName, Alias);
                var domain = new Uri("https://" + Domain);
                var getUri = new Uri(domain, uri);
                var session = RestImplementation.PostWithKey(getUri, ApiKey, ApiSecret, DateTime.UtcNow);

                // Return from the cmdlet 
                WriteObject(session);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex,
                            ex.GetType().Name,
                            ErrorCategory.InvalidOperation,
                            this.GetType().Name));
            }
        }
    }
}
