using System.Management.Automation;

namespace PowershellCmdlets
{
    [Cmdlet(VerbsSecurity.Unprotect, "FromInvalidTlsCertificates")]
    public class UnprotectFromInvalidTlsCertificatesCmdlet : Cmdlet
    {
        protected override void ProcessRecord()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;
        }
    }
}
