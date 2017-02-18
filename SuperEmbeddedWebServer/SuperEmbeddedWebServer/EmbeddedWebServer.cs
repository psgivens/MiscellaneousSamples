using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SuperWebSocket;
using SuperSocket.SocketBase;

namespace SuperEmbeddedWebServer
{
    public class EmbeddedWebServer
    {
        private readonly HttpListener _httpListener;
        private readonly WebSocketServer _socketServer;
        private readonly HttpRequestHandler _httpHandler;
        
        private EmbeddedWebServer(HttpRequestHandler handler)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                throw new NotSupportedException("SimpleListenerExample");
            }

            if (handler == null)
                throw new ArgumentNullException("handler");

            this._httpHandler = handler;

            // Setup the web sockets
            _socketServer = new WebSocketServer();
            var serverConfig = new SuperSocket.SocketBase.Config.ServerConfig
            {
                Name = "SuperWebSockets",
                Ip = "Any",
                Port = handler.WebSocketPort,
                Mode = SocketMode.Tcp
            };
            var rootConfig = new SuperSocket.SocketBase.Config.RootConfig();
            var factory = new SuperSocket.SocketEngine.SocketServerFactory();
            _socketServer.NewSessionConnected += _socketServer_NewSessionConnected;
            _socketServer.SessionClosed += _socketServer_SessionClosed;
            _socketServer.NewMessageReceived += _socketServer_NewMessageReceived;
            _socketServer.NewDataReceived += _socketServer_NewDataReceived;
            _socketServer.Setup(rootConfig, serverConfig, factory);
            _socketServer.Start();

            // Setup the web server
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(handler.WebPrefix);
            _httpListener.Start();
            StartListening();

            string chromeArgs = " --app=" + handler.WebPrefix;
            System.Diagnostics.Process.Start(handler.PathOfChromeExecutable, chromeArgs);
        }

        public void Stop()
        {
            _httpListener.Stop();
        }

        void _socketServer_NewSessionConnected(WebSocketSession session)
        {
            this._httpHandler.OnWebSocketConnection(this, session);
        }

        void _socketServer_SessionClosed(WebSocketSession session, CloseReason value)
        {
            this._httpHandler.OnWebSocketClosed(this, session, value);
        }

        void _socketServer_NewMessageReceived(WebSocketSession session, string value)
        {
            this._httpHandler.OnWebSocketMessageReceived(this, session, value);
        }

        void _socketServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            this._httpHandler.OnWebSocketDataReceived(this, session, value);
        }

        public void Post(DataPacket packet)
        {
            foreach (var session in _socketServer.GetAllSessions())
            {
                session.TrySend(Newtonsoft.Json.JsonConvert.SerializeObject(packet));
            }
        }

        public void Post(WebSocketSession session, DataPacket packet)
        {
            session.TrySend(Newtonsoft.Json.JsonConvert.SerializeObject(packet));
        }

        private async void StartListening()
        {
            Console.WriteLine("Listening...");

            while (_httpListener.IsListening)
            {
                var context = await _httpListener.GetContextAsync();
                if (context != null)
                {
                    Task.Run(() =>
                        {
                            _httpHandler.OnHttpRequest(this, context);
                        });
                }
            }
        }

        // This example requires the System and System.Net namespaces. 
        public static EmbeddedWebServer StartServer(HttpRequestHandler handler)
        {
            return new EmbeddedWebServer(handler);
        }
    }
}
