using SuperEmbeddedWebServer;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    class Program
    {
        private class Person : DataPacket
        {
            public Person() : base("Person") { }
            public string FirstName = "Johnny";
            public int[] Numbers = { 2, 3, 4, 5 };
        }

        private class MyHandler : HttpRequestHandler
        {            
            public MyHandler()
                : base("http://localhost:8080/", 
                    @"C:\Projects\ArchiveAndRetrieve\SelfHostedWebApplication\WebContent", 8089,
                    @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe") { }
             
            public override void OnWebSocketMessageReceived(EmbeddedWebServer server, WebSocketSession session, string value)
            {
                server.Post(new Person());
                base.OnWebSocketMessageReceived(server, session, value);
            }
        }

        static void Main(string[] args)
        {
            var server = SuperEmbeddedWebServer.EmbeddedWebServer.StartServer(
                new MyHandler());
            Console.ReadKey();
        }
    }
}
