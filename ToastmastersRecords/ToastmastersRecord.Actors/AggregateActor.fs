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
    
    let handleCommand (state, version) self cmdenv =
        cmdenv
        |> handle (CommandHandlers <| raiseVersioned self cmdenv) state
        |> Handler.Run version
        |> Async.Ignore
        |> Async.Start
        (state, version)

    let processEvent (state, version) envelope =               
        // Build current state
        let state' = buildState state [envelope.Item]

        // Command side persistence, record the event
        store.AppendEvent envelope

        // Query side persistence, record the state
        persist envelope.UserId envelope.StreamId state'
                
        eventSubject <! envelope

        (state', envelope.Version)

    let child streamId (mailbox:Actor<obj>) =
        let getState streamId =
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

        let rec loop stateAndVersion = actor {
            let! msg = mailbox.Receive ()
            return! 
                match msg with 
                | :? Envelope<'TCommand> as cmdenv -> cmdenv |> handleCommand stateAndVersion mailbox.Self 
                | :? Envelope<'TEvent> as evtenv ->   evtenv |> processEvent stateAndVersion
                | _ -> stateAndVersion
                |> loop
            }
            
        loop <| getState streamId
    
    fun (mailbox:Actor<obj>) ->
        let rec loop children = actor {
            let! msg = mailbox.Receive ()

            let getChild streamId =
                match children |> Map.tryFind streamId with
                | Some actor' -> (children,actor')
                | None ->   
                    let actorName = mailbox.Self.Path.Parent.Name + "_" + (streamId.ToString ())
                    let actor' = spawn mailbox actorName <| child streamId                                        
                    // TODO: Create timer for expiring cache
                    children |> Map.add streamId actor', actor'

            let forward env =
                let children', child = getChild env.StreamId
                child.Forward msg
                children'

            return! 
                match msg with 
                | :? Envelope<'TCommand> as env -> forward env
                | :? Envelope<'TEvent> as env -> forward env
                | _ -> children
                |> loop
        }
        loop Map.empty<StreamId, IActorRef>



