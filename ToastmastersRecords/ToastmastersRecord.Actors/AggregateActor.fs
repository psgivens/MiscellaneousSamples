[<RequireQualifiedAccess>]
module ToastmastersRecord.Actors.AggregateActor

open Akka.Actor
open Akka.FSharp

open ToastmastersRecord.Domain.CommandHandlers
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.Infrastructure.Envelope

let create<'TState, 'TCommand, 'TEvent> 
    (   eventSubject:IActorRef,
        invalidMessageSubject:IActorRef,
        store:IEventStore<'TEvent>, 
        buildState:'TState option -> 'TEvent list -> 'TState option,
        handle:'TState option -> Envelope<'TCommand> -> CommandHandlers<'TEvent, Version> -> CommandHandlerFunction<Version>,
        persist:UserId -> StreamId -> 'TState option -> unit) =

    let raiseVersioned (self:IActorRef) cmdenv (version:Version) nevent =
        let newVersion = incrementVersion version
        // publish new event
        let envelope = 
            envelopWithDefaults 
                cmdenv.UserId 
                cmdenv.TransactionId 
                cmdenv.StreamId 
                newVersion
                nevent

        envelope |> self.Tell        
        newVersion

    let getState states streamId =
        match states |> Map.tryFind streamId with
        | Some values -> values
        | None -> 
            let events = 
                store.GetEvents streamId 
                // Crudely remove concurrency errors
                // TODO: Devise error correction mechanism
                |> List.distinctBy (fun e -> e.Version)
                
            let version = 
                if events |> List.isEmpty then Version.box 0s
                else events |> List.last |> (fun e -> e.Version)

            // Build current state
            let state = buildState None (events |> List.map unpack)
            state, version
    
    
    fun (mailbox:Actor<obj>) ->
        let rec loop states = actor {
            let! msg = mailbox.Receive ()
            match msg with 
            | :? Envelope<'TCommand> as cmdenv -> 
                let state, version = getState states cmdenv.StreamId
            
                raiseVersioned mailbox.Self cmdenv
                |> CommandHandlers 
                |> handle state cmdenv
                |> Handler.Run version
                |> Async.Ignore
                |> Async.Start

                return! loop states 

            | :? Envelope<'TEvent> as envelope -> 
                let state, version = getState states envelope.StreamId
              
                // Build current state
                let state' = buildState state [envelope.Item]

                // Command side persistence, record the event
                store.AppendEvent envelope         

                // Query side persistence, record the state
                persist envelope.UserId envelope.StreamId state'
                
                eventSubject <! envelope

                // TODO: Create timer for expiring cache
                return! loop (states |> Map.add envelope.StreamId (state', version))

            | _ -> 
                return! loop states
        }
        loop Map.empty<StreamId, 'TState option * Version>



