using System;
using System.Collections;
using System.Management.Automation;

namespace CsodRestClients.Cmdlets
{
    [Cmdlet(VerbsSecurity.Protect, "CsodRestHeaders")]
    [OutputType(typeof(Hashtable))]
    public class ProtectCsodRestHeadersCmdlet : Cmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "Signed Headers")]
        [Parameter(Mandatory = true, ParameterSetName = "Basic Auth")]
        public TokenSession Session { get; set; }
        
        [Parameter(Mandatory = true, ParameterSetName = "Signed Headers")]
        public string Url { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Signed Headers")]
        public DateTime? DateTime { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Signed Headers")]
        [ValidateSet("Default", "Delete", "Get", "Head", "Merge", "Options", "Patch", "Post", "Put", "Trace")]
        public string Method { get; set; }

        [Parameter(Mandatory = true, 
            ValueFromPipeline = true,
            ParameterSetName = "Basic Auth")]
        public string Base64AuthCreds { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Base64AuthCreds))
                {
                    var hash = new Hashtable();
                    hash["x-csod-session-token"] = Session.Token;
                    hash["Authorization"] = string.Format("Basic {0}", Base64AuthCreds);

                    WriteObject(hash);
                }
                else
                {
                    var time = DateTime.HasValue ? DateTime.Value : System.DateTime.UtcNow;
                    var uri = new Uri(Url);
                    var method = Method.ToUpper();

                    var hash = new Hashtable();
                    hash["x-csod-date"] = time.ToString("yyyy-MM-ddTHH:mm:ss.000");
                    hash["x-csod-session-token"] = Session.Token;

                    var stringToSign = RestImplementation.ConstructStringToSign(method, hash, uri.AbsolutePath);
                    var sig = RestImplementation.SignString512(stringToSign, Session.Secret);
                    hash["x-csod-signature"] = sig;

                    WriteObject(hash);
                }
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
