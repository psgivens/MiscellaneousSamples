[<RequireQualifiedAccess>]
module ToastmastersRecord.Actors.AggregateActor

open Akka.Actor
open Akka.FSharp

open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.Infrastructure.Envelope
open ToastmastersRecord.Domain.CommandHandler


let create<'TState, 'TCommand, 'TEvent> 
    (   eventSubject:IActorRef,
        invalidMessageSubject:IActorRef,
        store:IEventStore<'TEvent>, 
        buildState:'TState option -> 'TEvent list -> 'TState option,
        handle:CommandHandlers<'TEvent, 'TState> ->'TState option -> Envelope<'TCommand> -> CommandHandlerFunction<'TEvent, 'TState>,
        persist:UserId -> StreamId -> 'TState option -> unit) =

    let raiseVersioned (self:IActorRef) cmdenv state version nevent =             
        // publish new event
        let envelope = 
            envelopWithDefaults 
                cmdenv.UserId 
                cmdenv.TransactionId 
                cmdenv.StreamId 
                (Version.box <| version + 1s) 
                nevent

        store.AppendEvent envelope 

        // Build and persist current state
        let state' = buildState state [nevent]
        persist cmdenv.UserId cmdenv.StreamId state'               
        self <! envelope                
        state'
    
    fun (mailbox:Actor<obj>) ->
        let rec loop () = actor {
            let! msg = mailbox.Receive ()
            match msg with 
            | :? Envelope<'TCommand> as cmdenv -> 
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
            
                let handlers = CommandHandlers <| raiseVersioned mailbox.Self cmdenv

                (version, [], state) 
                |> handle handlers state cmdenv 
                |> Async.RunSynchronously
                |> ignore

            | :? Envelope<'TEvent> as envelope -> eventSubject <! envelope
            | _ -> ()
            return! loop ()
        }
        loop ()



