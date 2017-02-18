
// Open source web-sockets library
#r @"C:\Projects\SuperWebSockets_SuperSockets\SuperWebSocket\bin\Release\SuperWebSocket.dll"

// Open source socket libraries used by SuperWebSockets
// -- The nuget project seemed to have dependency issues. I had to download both SuperWebSockets 
// -- and SuperSockets, create a composite solution and build to get the dependencies correct.
#r @"C:\Projects\SuperWebSockets_SuperSockets\SuperWebSocket\bin\Release\SuperSocket.SocketBase.dll"
#r @"C:\Projects\SuperWebSockets_SuperSockets\SocketEngine\bin\Release\SuperSocket.SocketEngine.dll"
#r @"C:\Projects\SuperWebSockets_SuperSockets\SuperWebSocket\bin\Release\SuperSocket.Common.dll"

// Crude web server - serves http requests as well as services web-socket requests
#r @"C:\Projects\SuperEmbeddedWebServer\SuperEmbeddedWebServer\bin\Debug\SuperEmbeddedWebServer.dll"

// SuperEmbeddedWebServer wrapper to make it friendlier to the F# interactive.
#r @"C:\Projects\SuperEmbeddedWebServer\FSharp.EmbededWebServer\bin\Debug\FSharp.EmbededWebServer.dll"

// Our web server uses log4net
#r @"C:\Projects\SuperEmbeddedWebServer\packages\log4net.2.0.3\lib\net40-full\log4net.dll"


open SuperEmbeddedWebServer
open SuperWebSocket

let handler = new FSharp.EmbededWebServer.FSharpHttpHandler()

let server = SuperEmbeddedWebServer.EmbeddedWebServer.StartServer(handler);

(* ****************************************************************** *)
(* ************ Play around with the code below ********************* *)
(* ****************************************************************** *)

type PersonDataPacket(values: int list) =
    inherit DataPacket("Person2")
    member this.FirstName with get() = "Tony"
    member this.Numbers with get()   = values

handler.WebSocketMessageReceivedAction <- (fun (server, session, value) ->
    server.Post(new PersonDataPacket([7;10;25;52;42;8;2;19])))

handler.WebSocketMessageReceivedAction <- (fun (server, session, value) ->
    server.Post(new PersonDataPacket([1;2;3;4;5;6;7])))

server.Post(new PersonDataPacket([50;40;30;20;10;2;50;40;30;20;10;2]))    

server.Post(new PersonDataPacket(
    [50;40;30;20;10;2;50;40;30;20;10;2] 
    |> List.rev))    

let newData = 
    [50;40;30;20;10;2;50;40;30;20;10;2]
    |> List.map (fun value ->
        value * 2)

server.Post(new PersonDataPacket(newData |> List.map (fun x -> (x % 3) * 12)))    



