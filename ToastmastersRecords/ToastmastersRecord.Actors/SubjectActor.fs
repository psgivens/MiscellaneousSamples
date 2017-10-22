module ToastmastersRecord.Actors.SubjectActor

open Akka.Actor
open Akka.FSharp
open ToastmastersRecord.Domain.Infrastructure

type SubjectAction =
    | Msg of System.Object
    | Subscribe of IActorRef
    | Unsubscribe of IActorRef

let create<'TMessage> system name =     
    let subject' = spawn system name (fun mailbox -> 
        let rec loop subscribers = actor {
            let! message = mailbox.Receive ()
            match message with 
            | Subscribe actor -> 
                return! loop 
                    <|  match subscribers 
                            |> List.tryFind (fun (a:IActorRef) -> 
                                actor.Path = a.Path) 
                            with
                        | None -> actor::subscribers
                        | Some(_) -> subscribers
            | Unsubscribe actor -> 
                return! loop 
                    (subscribers 
                     |> List.filter (fun item -> 
                        item <> actor))        
            | Msg msg -> 
                subscribers |> List.iter (fun actor -> actor.Tell msg)
                return! loop subscribers
        }        
        loop []) 
    let post = 
        spawn system (name+"post") (fun mailbox ->
            let rec loop () = actor {
                let! message = mailbox.Receive ()
                subject' <! Msg message
                return! loop ()
            }
            loop ())
    subject', post


let subscribeTo (events:IActorRef)  =
    Subscribe >> events.Tell

let unsubscribeFrom (events:IActorRef)  =
    Unsubscribe >> events.Tell
