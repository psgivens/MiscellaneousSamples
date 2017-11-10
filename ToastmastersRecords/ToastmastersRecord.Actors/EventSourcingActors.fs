﻿[<RequireQualifiedAccess>]
module ToastmastersRecord.Actors.EventSourcingActors

open Akka.Actor
open Akka.FSharp

open ToastmastersRecord.Domain

open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.Infrastructure.Envelope
open ToastmastersRecord.Domain.CommandHandler
open ToastmastersRecord.Actors
open ToastmastersRecord.Actors.Infrastructure

let spawn<'TCommand, 'TEvent, 'TState>
   (sys,
    name,
    eventStore,
    buildState:'TState option -> 'TEvent list -> 'TState option,
    handle:CommandHandlers<'TEvent, Version> -> 'TState option -> Envelope<'TCommand> -> CommandHandlerFunction<Version>,
    persist:UserId -> StreamId -> 'TState option -> unit) :ActorIO<'TCommand> = 

    // Create a subject so that the next step can subscribe. 
    let persistEventSubject = SubjectActor.spawn sys (name + "_Events")
    let errorSubject = SubjectActor.spawn sys (name + "_Errors")

    // Create member management actors: aggregate !> persist !> subjects
//    let persistingActor =
//        PersistanceActor.create
//            (persistEventSubject,
//             errorSubject,
//             eventStore,
//             buildState,
//             persist)
//        |> spawn sys (name + "_PersistingActor")
    let aggregateActor =
        AggregateActor.create
            (persistEventSubject,
             errorSubject,
             eventStore,
             buildState,
             handle,
             persist
             )      
        |> spawn sys (name + "_AggregateActor")
    { Tell=aggregateActor.Tell; 
      Actor=aggregateActor;
      Events=persistEventSubject;
      Errors=errorSubject }
