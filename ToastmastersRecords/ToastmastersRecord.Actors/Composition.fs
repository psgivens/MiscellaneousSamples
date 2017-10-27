module ToastmastersRecord.Actors.Composition

open Akka.Actor
open Akka.FSharp

open ToastmastersRecord.Domain

open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.Infrastructure.Envelope
open ToastmastersRecord.Domain.CommandHandler
open ToastmastersRecord.Actors

type ActorIO<'a> = { Tell:Envelope<'a> -> unit; Events:IActorRef; Errors:IActorRef }

let spawnEventSourcingActors 
   (sys,
    name,
    eventStore,
    buildState:'TState option -> 'TEvent list -> 'TState option,
    handle:CommandHandlers<'TEvent> -> 'TState option -> Envelope<'TCommand> -> CommandHandlerFunction<'TEvent>,
    persist:UserId -> StreamId -> 'TState option -> unit) = 

    // Create a subject so that the next step can subscribe. 
    let persistEventSubject, persistEventPoster = SubjectActor.create<'TEvent> sys (name + "_Events")
    let errorSubject, errorPoster = SubjectActor.create<'TEvent> sys (name + "_Errors")

    // Create member management actors: aggregate !> persist !> subjects
    let persistingActor =
        PersistanceActor.create
            (persistEventPoster ,
             errorPoster,
             eventStore,
             buildState,
             persist)
        |> spawn sys (name + "_PersistingActor")
    let aggregateActor =
        AggregateActor.create
            (persistingActor,
             errorPoster,
             eventStore,
             buildState,
             handle
             )      
        |> spawn sys (name + "_AggregateActor")
    { Tell=aggregateActor.Tell; 
      Events=persistEventSubject;
      Errors=errorSubject }

let spawnCrudPersistActors<'TState> 
   (sys,
    name,
    persist:UserId -> StreamId -> Envelope<'TState> option -> unit) = 
    // Create a subject so that the next step can subscribe. 
   let persistEntitySubject, persistEntityPoster = SubjectActor.create<'TState> sys (name + "_Events")
   let errorSubject, errorPoster = SubjectActor.create<'TState> sys (name + "_Errors")
   let messagePersisting = 
       CrudMessagePersistanceActor.create<'TState>
           (persistEntityPoster,
            errorPoster,
            persist)
       |> spawn sys (name + "_PersistingActor")

   { Tell=fun (env:Envelope<'TState>) -> env |> messagePersisting.Tell; 
     Events=persistEntitySubject;
     Errors=errorSubject }


let spawnRequestReplyConditionalActor<'TCommand,'TEvent> inFilter outFilter sys name (actors:ActorIO<'TCommand>) =
    let actor = spawn sys (name + "_requestreply") <| fun (mailbox:Actor<obj>) ->
        let rec loop senders = actor {
            let! msg = mailbox.Receive ()
            match msg with
            | :? Envelope<'TCommand> as cmdenv ->        
                if inFilter cmdenv then                          
                    cmdenv |> actors.Tell
                    return! loop (senders |> Map.add cmdenv.StreamId (mailbox.Sender ()))                
            | :? Envelope<'TEvent> as evtenv ->
                if outFilter evtenv then 
                    match senders |> Map.tryFind evtenv.StreamId with
                    | Some(sender) -> 
                        sender <! evtenv
                        return! loop (senders |> Map.remove evtenv.StreamId)
                    | None -> ()
            | :? string as value ->
                match value with 
                | "Unsubscribe" -> mailbox.Self |> SubjectActor.unsubscribeFrom actors.Events
                | _ -> ()
            | _ -> ()
            return! loop senders
        }
        loop Map.empty<StreamId, IActorRef> 
    actor |> SubjectActor.subscribeTo actors.Events
    actor

let spawnRequestReplyActor<'TCommand,'TEvent> sys name (actors:ActorIO<'TCommand>) =
    spawnRequestReplyConditionalActor<'TCommand,'TEvent> 
        (fun x -> true)
        (fun x -> true)
        sys name (actors:ActorIO<'TCommand>) 
