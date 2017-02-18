using SuperSocket.SocketBase;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuperEmbeddedWebServer
{
    public class HttpHandler
    {
        private static readonly Dictionary<string, string> contentTypes = new Dictionary<string, string>
            {
                { ".css", "text/css" }, { ".html", "text/html"}, { ".js", "text/javascript"}, 
                { ".gif", "image/gif"}, { ".png", "image/png"}, { ".jpg", "image/jpeg"}, 
                { ".mp3", "audio/mpeg"}, { ".wav", "audio/wav"}, { ".mpg", "video/mpeg" }
            };

        public HttpHandler(
            string webPrefix,
            string localWebRoot,
            int webSocketPort,
            string pathToChromeExecutable)
        {
            this.WebPrefix = webPrefix;
            this.LocalWebRoot = localWebRoot;
            this.WebSocketPort = webSocketPort;
            this.PathOfChromeExecutable = pathToChromeExecutable;
        }

        public string LocalWebRoot { get; private set; }
        public string WebPrefix { get; private set; }
        public int WebSocketPort { get; private set; }
        public string PathOfChromeExecutable { get; private set; }
        public HttpHandler Handler { get; private set; }

        public virtual void OnHttpRequest(EmbeddedWebServer server, HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;

            // Obtain a response object.                            
            HttpListenerResponse response = context.Response;

            var resourcePath = context.Request.Url.LocalPath;
            resourcePath = resourcePath == "/" ? "/Index.html" : resourcePath;
            var resourceName = "SuperEmbeddedWebServer.WebContent" + resourcePath.Replace('/', '.');
            Assembly thisAssembly = Assembly.GetExecutingAssembly();

            string resourceText = string.Empty;
            byte[] resourceBuffer = null;
            using (Stream stream = thisAssembly.GetManifestResourceStream(resourceName))
            using (MemoryStream ms = new MemoryStream())
            {
                if (stream != null)
                {
                    stream.CopyTo(ms);
                    resourceBuffer = ms.ToArray();
                }
            }
            
            var filePath = Path.Combine(LocalWebRoot, resourcePath.Substring(1));

            if (resourceBuffer != null)
            {
                string ext = Path.GetExtension(resourcePath);
                string type = contentTypes[ext];
                var buffer = resourceBuffer;
                response.ContentType = type;

                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            else
            {
                var buffer = Encoding.UTF8.GetBytes(String.Format("File not found {0}", filePath));
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            
        }
        public virtual void OnWebSocketConnection(EmbeddedWebServer server, WebSocketSession session)
        {
        }
        public virtual void OnWebSocketClosed(EmbeddedWebServer server, WebSocketSession session, CloseReason value)
        {
        }
        public virtual void OnWebSocketMessageReceived(EmbeddedWebServer server, WebSocketSession session, string value)
        {
        }
        public virtual void OnWebSocketDataReceived(EmbeddedWebServer server, WebSocketSession session, byte[] value)
        {
        }
    }

}
