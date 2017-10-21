module ToastmastersRecord.Actors.Composition

open Akka.Actor
open Akka.FSharp

open ToastmastersRecord.Domain

open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.Infrastructure.Envelope
open ToastmastersRecord.Actors

type ActorIO = { In:IActorRef; Events:IActorRef; Errors:IActorRef }

let spawnEventSourcingActors 
   (sys,
    name,
    eventStore,
    buildState:'TState option -> 'TEvent list -> 'TState option,
    handle:('TEvent -> ('TEvent -> unit) seq)  -> 'TState option -> Envelope<'TCommand> -> unit,
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
    { In=aggregateActor; 
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

   { In=messagePersisting; 
     Events=persistEntitySubject;
     Errors=errorSubject }

let spawnRequestReplyActor<'TCommand,'TEvent> sys name (actors:ActorIO) =
    let actor = spawn sys (name + "_requestreply") <| fun (mailbox:Actor<obj>) ->
        let rec loop senders = actor {
            let! msg = mailbox.Receive ()
            match msg with
            | :? Envelope<'TCommand> as cmdenv ->                                 
                actors.In <! cmdenv
                return! loop (senders |> Map.add cmdenv.StreamId (mailbox.Sender ()))
                
            | :? Envelope<'TEvent> as evtenv ->
                senders 
                |> Map.find evtenv.StreamId
                |> fun sender -> sender <! evtenv
                return! loop (senders |> Map.remove evtenv.StreamId)
            | _ -> return! loop senders
        }
        loop Map.empty<StreamId, IActorRef> 
    actor |> SubjectActor.subscribeTo actors.Events
    actor


