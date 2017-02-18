namespace FSharp.EmbededWebServer

open SuperEmbeddedWebServer
open SuperWebSocket

type FSharpHttpHandler() =
    inherit HttpRequestHandler("http://localhost:8080/", 
                    @"C:\Projects\ArchiveAndRetrieve\SelfHostedWebApplication\WebContent", 
                    8089,
                    @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe") 
    //member val webSocketMessageReceivedAction = ((EmbeddedWebServer, WebSocketSession, string) -> ())
    member val WebSocketMessageReceivedAction 
        = (fun ((server:EmbeddedWebServer), (session:WebSocketSession), (value:string))-> 
            (server,session,value) |> ignore) 
            with get,set
    override this.OnWebSocketMessageReceived((server:EmbeddedWebServer), (session:WebSocketSession), (value:string)) =    
        (server, session, value) |> this.WebSocketMessageReceivedAction
        base.OnWebSocketMessageReceived(server, session, value);         
