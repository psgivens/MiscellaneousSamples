using System;
using System.Management.Automation;
using System.Text;
using System.Windows;

namespace PowershellCmdlets
{
    [Cmdlet(VerbsCommon.Get, "Base64Creds")]
    [OutputType(typeof(string))]
    public class GetBase64CredsCmdlet : Cmdlet
    {
        [Parameter]
        [ValidateSet("AuthBasic", "Basic", "EncOnly")]
        public string OutputType { get; set; }

        protected override void ProcessRecord()
        {
            string creds = string.Empty;
            var application = Application.Current ?? new Application();

            var @event = new System.Threading.AutoResetEvent(false);

            application.Dispatcher.Invoke(() =>
            {
                var mainWindow = new MainWindow();
                mainWindow.ShowDialog();

                var pair = String.Format("{0}:{1}", mainWindow.UserName, mainWindow.Password);
                creds = Convert.ToBase64String(Encoding.ASCII.GetBytes(pair));

                @event.Set();
            });

            @event.WaitOne(60 * 1000);
            var outputType = OutputType ?? "enconly";
            switch(outputType.ToLower ())
            {
                case "authbasic":
                    WriteObject("Authorization: Basic " + creds);
                    break;
                case "basic":
                    WriteObject("Basic " + creds);
                    break;
                case "enconly":
                    WriteObject(creds);
                    break;
                default:
                    WriteObject(creds);
                    break;
            }

            base.ProcessRecord();
        }
    }
}
