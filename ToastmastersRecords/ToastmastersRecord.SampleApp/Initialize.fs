module ToastmastersRecord.SampleApp.Initialize

open Akka.Actor
open Akka.FSharp

open ToastmastersRecord.Domain
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.MemberManagement
open ToastmastersRecord.Domain.RoleRequests
open ToastmastersRecord.Domain.MemberMessages
open ToastmastersRecord.Domain.RolePlacements 
open ToastmastersRecord.Domain.ClubMeetings
open ToastmastersRecord.Actors

open ToastmastersRecord.Domain.Persistence.ToastmastersEventStore
open ToastmastersRecord.Actors.Infrastructure
open ToastmastersRecord.Actors

open System.Threading.Tasks

type ActorGroups = {
    MemberManagementActors:ActorIO<MemberManagementCommand>
    MessageActors:ActorIO<MemberMessageCommand>
    RoleRequestActors:ActorIO<RoleRequestCommand>
    RolePlacementActors:ActorIO<RolePlacementCommand>
    ClubMeetingActors:ActorIO<ClubMeetingCommand>
    }

let composeActors system =
    // Create member management actors
    let memberManagementActors = 
        EventSourcingActors.spawnEventSourcingActors 
            (system,
             "memberManagement", 
             MemberManagementEventStore (),
             buildState MemberManagement.evolve,
             MemberManagement.handle,
             Persistence.MemberManagement.persist)    

    let messageActors = 
        CrudMessagePersistanceActor.spawn<MemberMessageCommand>
            (system, 
             "memberMessage", 
             Persistence.MemberMessages.persist)

    // Create role request actors
    let roleRequestActors =
        EventSourcingActors.spawnEventSourcingActors
            (system,
             "roleRequests",
             RoleRequestEventStore (),
             buildState RoleRequests.evolve,
             RoleRequests.handle,
             Persistence.RoleRequests.persist)

    // Create role request actors
    let rolePlacementActors =
        EventSourcingActors.spawnEventSourcingActors
            (system,
             "rolePlacements",
             RolePlacementEventStore (),
             buildState RolePlacements.evolve,
             RolePlacements.handle,
             Persistence.RolePlacements.persist)   

    let placementRequestReplyCreate = 
        RequestReplyActor.spawnRequestReplyConditionalActor<RolePlacementCommand,RolePlacementEvent> 
            (fun cmd -> true)
            (fun evt -> 
                match evt.Item with
                | RolePlacementEvent.Opened _ -> true
                | _ -> false)
            system "rolePlacement_create" rolePlacementActors
    let createRolePlacement meetingEnv roleTypeId = 
        ((roleTypeId, MeetingId.box <| StreamId.unbox meetingEnv.StreamId)
        |> RolePlacementCommand.Open
        |> envelopWithDefaults
            (meetingEnv.UserId)
            (meetingEnv.TransactionId)
            (StreamId.create ())
            (Version.box 0s))
        |> placementRequestReplyCreate.Ask

    let placementRequestReplyCancel = 
        RequestReplyActor.spawnRequestReplyConditionalActor<RolePlacementCommand,RolePlacementEvent> 
            (fun cmd -> true)
            (fun evt -> 
                match evt.Item with
                | RolePlacementEvent.Canceled _ -> true
                | _ -> false)
            system "rolePlacement_cancel" rolePlacementActors

    let cancelRolePlacement findMeetingPlacements meetingEnv =         
        findMeetingPlacements meetingEnv.Id 
        |> List.map (fun placement ->
            RolePlacementCommand.Cancel
            |> envelopWithDefaults
                (meetingEnv.UserId)
                (meetingEnv.TransactionId)
                (StreamId.create ())
                (Version.box 0s)
            |> placementRequestReplyCancel.Ask
            :> Task)
        |> List.toArray
        |> Task.WhenAll
        
    // Create member management actors
    let clubMeetingActors = 
        EventSourcingActors.spawnEventSourcingActors 
            (system,
             "clubMeetings", 
             ClubMeetingEventStore (),
             buildState ClubMeetings.evolve,
             (ClubMeetings.handle {
                RoleActions.createRole=createRolePlacement
                RoleActions.cancelRoles=cancelRolePlacement Persistence.RolePlacements.findMeetingPlacements}),
             Persistence.ClubMeetings.persist)    
             
    { MemberManagementActors=memberManagementActors
      MessageActors=messageActors
      RoleRequestActors=roleRequestActors
      RolePlacementActors=rolePlacementActors
      ClubMeetingActors=clubMeetingActors
    }
