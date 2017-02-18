using PhillipScottGivens.WebDownloader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.WebDownloader.Services.Tests
{
    class SuccessDownloaderClient : IDownloaderClient
    {
        public async Task<string> DownloadStringAsync(string address)
        {
            // Try to simulate a network connection
            System.Threading.Thread.Sleep(200);

            // Return some text from one of my random blogs. 
            return "eXtensible Application Markup Language (XAML) is an object initialization language.           "
            + "It has become synonymous with the frameworks that use it, but it is not the frameworks that        "
            + "use it. It is an implementation of the eXtensible Markup Language (XML) and is used by             "
            + "Windows Presentation Framework (WPF), Silverlight, Windows Workflow Foundation (WF) and now        "
            + "Metro (.net and native).                                                                           "
            + "This article is meant to be a XAML primer devoid of the technologies that use it. It is meant      "
            + "to help developers like me who find benefit in learning one technology in isolation from another.  "
            + "Everything in this article can be used with a reference to System.Xaml.dll and does not require    "
            + "a reference to the WPF asemblies. The source code for this article can be found here:              "
            + "Download XamlExample.7z                                                                            "
            + "Here is a table of contents:                                                                       "
            + "Constructors and Property Setters                                                                  "
            + "Static Setters                                                                                     "
            + "TypeConverter's                                                                                    "
            + "Markup Extensions                                                                                  "
            + "x:Name                                                                                             "
            + "x:Reference                                                                                        "
            + "Inheritance                                                                                        "
            + "Summary                                                                                            "
            + "References                                                                                         "
            + "Constructors and Property Setters                                                                  "
            + "The objects that XAML initializes can be any .net objects whether they are user interface, windows "
            + "workflow activities or anything else that you would like to hydrate from file. XAML can construct  "
            + "objects and assign object properties. For the rest of this article let us define a simple class    "
            + "called SampleClass.                                                                                ";
        }

        public void Dispose()
        {
        }
    }
}
