module ToastmastersRecord.Actors.CrudMessagePersistanceActor

open Akka.Actor
open Akka.FSharp

open ToastmastersRecord.Actors.SubjectActor
open ToastmastersRecord.Domain.Infrastructure

let create<'TCommand> 
    (   eventSubject:IActorRef,
        errorSubject:IActorRef,
        persist:UserId -> StreamId -> Envelope<'TCommand> option -> unit) =

    let persistEntity (mailbox:Actor<Envelope<'TCommand>>) (envelope:Envelope<'TCommand>) =
        try
            persist
                envelope.UserId
                envelope.StreamId
                (Some(envelope))

            envelope |> eventSubject.Tell
        with
            | ex -> errorSubject <! ex
                                        
    actorOf2 persistEntity

