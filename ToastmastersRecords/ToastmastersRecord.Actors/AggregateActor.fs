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
        handle:CommandHandlers<'TEvent, Version> -> 'TState option -> Envelope<'TCommand> -> CommandHandlerFunction<Version>,
        persist:UserId -> StreamId -> 'TState option -> unit) =

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

    let raiseVersioned (self:IActorRef) cmdenv (version:Version) event =
        let newVersion = incrementVersion version
        // publish new event
        let envelope = 
            envelopWithDefaults 
                cmdenv.UserId 
                cmdenv.TransactionId 
                cmdenv.StreamId 
                newVersion
                event

        envelope |> self.Tell        
        newVersion
    
    let handleCommand states self cmdenv =
        let state, version = getState states cmdenv.StreamId

        cmdenv
        |> handle (CommandHandlers <| raiseVersioned self cmdenv) state
        |> Handler.Run version
        |> Async.Ignore
        |> Async.Start

        states 

    let processEvent states envelope = 
        let state, version = getState states envelope.StreamId
              
        // Build current state
        let state' = buildState state [envelope.Item]

        // Command side persistence, record the event
        store.AppendEvent envelope

        // Query side persistence, record the state
        persist envelope.UserId envelope.StreamId state'
                
        eventSubject <! envelope

        // TODO: Create timer for expiring cache
        states |> Map.add envelope.StreamId (state', envelope.Version)
    
    fun (mailbox:Actor<obj>) ->
        let rec loop states = actor {
            let! msg = mailbox.Receive ()
            return! 
                match msg with 
                | :? Envelope<'TCommand> as cmdenv -> cmdenv |> handleCommand states mailbox.Self 
                | :? Envelope<'TEvent> as envelope -> envelope |> processEvent states 
                | _ -> states
                |> loop
        }
        loop Map.empty<StreamId, 'TState option * Version>



