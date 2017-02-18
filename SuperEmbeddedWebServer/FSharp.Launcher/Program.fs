// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open SuperEmbeddedWebServer
open SuperWebSocket

type Person() =
    inherit DataPacket("Person")
    member this.FirstName with get() = "Tony"
    member this.Numbers with get()   = [2;3;4;5;6]
        
type Person2(values: int list) =
    inherit DataPacket("Person2")
    member this.FirstName with get() = "Tony"
    member this.Numbers with get()   = values

type MyHandler() =
    inherit HttpRequestHandler("http://localhost:8080/", 
                    @"C:\Projects\ArchiveAndRetrieve\SelfHostedWebApplication\WebContent", 
                    8089,
                    @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe") 
    override this.OnWebSocketMessageReceived( (server:EmbeddedWebServer), (session:WebSocketSession), (value:string)) =    
                server.Post(new Person());
                base.OnWebSocketMessageReceived(server, session, value);         
    
[<EntryPoint>]
let main argv = 
    let server = SuperEmbeddedWebServer.EmbeddedWebServer.StartServer(new MyHandler());
    printfn "%A" argv
    System.Console.ReadKey() |> ignore
    server.Post(new Person2([7;10;15;3;80;2;19]))    
    System.Console.ReadKey() |> ignore
    0 // return an integer exit code
