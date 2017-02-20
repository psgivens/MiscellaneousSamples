module ForonoiProcessor
open ForonoiSweep
open Microsoft.FSharp.Control

type Agent<'Type> = Control.MailboxProcessor<'Type>

let processForonoi (coords:(int*int) list) = 
    let heap = 
        coords 
        |> List.fold (fun acc coord ->
            acc |> insert (Vertex(coord))) startNode
    ()