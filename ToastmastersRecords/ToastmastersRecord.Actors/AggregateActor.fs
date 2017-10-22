module ToastmastersRecord.Actors.AggregateActor

open Akka.Actor
open Akka.FSharp
//open ToastmastersRecord.Actors.SubjectActor

open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.Infrastructure.Envelope

let create<'TState, 'TCommand, 'TEvent> 
    (   eventSubject:IActorRef,
        invalidMessageSubject:IActorRef,
        store:IEventStore<Envelope<'TEvent>>, 
        buildState:'TState option -> 'TEvent list -> 'TState option,
        handle:(('TEvent -> unit) seq) ->'TState option -> Envelope<'TCommand> -> unit) =         
    
    let processMessage (mailbox:Actor<Envelope<'TCommand>>) cmdenv=
        let events = 
            store.GetEvents cmdenv.StreamId 
            // Crudely remove concurrency errors
            // TODO: Devise error correction mechanism
            |> List.distinctBy (fun e -> e.Version)
                
        let version = 
            if events |> List.isEmpty then 0s
            else events |> List.last |> (fun e -> Version.unbox e.Version)

        // Build current state
        let state = buildState None (events |> List.map unpack)
            
        try
            // TODO: Increase the version with every call
            let raiseVersioned version nevent = 
            
                // publish new event
                let envelope = 
                    envelopWithDefaults 
                        cmdenv.UserId 
                        cmdenv.TransactionId 
                        cmdenv.StreamId 
                        (Version.box <| version + 1s) 
                        nevent

                store.AppendEvent cmdenv.StreamId envelope 
                eventSubject <! envelope

            let raise =
                version + 1s
                |> Seq.unfold (fun i ->
                    Some(raiseVersioned i, i+1s)
                    )

            // 'handle' current cmd
            handle raise state cmdenv

        with
        | :? InvalidEvent as ex -> invalidMessageSubject <! ex
        | :? InvalidCommand as ex -> invalidMessageSubject <! ex
    actorOf2 processMessage

