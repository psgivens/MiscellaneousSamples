module ToastmastersRecord.SampleApp.IngestMeetings

open FSharp.Data
open Akka.Actor
open Akka.FSharp

open ToastmastersRecord.Actors

open ToastmastersRecord.Domain
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.RolePlacements
open ToastmastersRecord.Domain.ClubMeetings
open ToastmastersRecord.SampleApp.Infrastructure
open ToastmastersRecord.SampleApp.Initialize

let createMeetings system userId actorGroups =
    // Create an request-reply actor that we can wait on. 
    let meetingRequestReplyCreate = 
        RequestReplyActor.spawnRequestReplyConditionalActor<ClubMeetingCommand,ClubMeetingEvent> 
            (fun x -> true)
            (fun x -> x.Item = Initialized)
            system "clubMeeting_initialized" actorGroups.ClubMeetingActors

    // Create a sequence of dates
    System.DateTime.Parse("07/11/2017")           
    |> Seq.unfold (fun date -> 
        if date < System.DateTime.Now then Some(date, date.AddDays 7.0)
        else None)

    // Create a meeting and wait for creation
    |> Seq.map (fun date -> 
        date
        |> ClubMeetings.ClubMeetingCommand.Create
        |> envelopWithDefaults
            (userId)
            (TransId.create ())
            (StreamId.create ())
            (Version.box 0s)
        |> meetingRequestReplyCreate.Ask
        |> Async.AwaitTask)

    // Wait for completion of all meeting creations
    |> Async.Parallel
    |> Async.Ignore
    |> Async.RunSynchronously

    // Unsubscribe and stop request-reply
    meetingRequestReplyCreate <! "Unsubscribe"

    // Create a request-reply which waits for the canceled event
    let meetingRequestReplyCanceled = 
        RequestReplyActor.spawnRequestReplyConditionalActor<ClubMeetingCommand,ClubMeetingEvent> 
            (fun x -> true)
            (fun x -> x.Item = ClubMeetingEvent.Canceled)
            system "clubMeeting_canceled" actorGroups.ClubMeetingActors

//    ["9/12/2017";"9/26/2017"]
//    |> List.map System.DateTime.Parse
//    |> List.map Persistence.ClubMeetings.findByDate
//    |> Seq.map (fun meeting -> 
//        ClubMeetings.ClubMeetingCommand.Cancel
//        |> envelopWithDefaults
//            (userId)
//            (TransId.create ())
//            (StreamId.box meeting.Id)
//            (Version.box 0s)
//        |> meetingRequestReplyCanceled.Ask
//        |> fun t -> t :> Task)
//    |> Seq.toArray
//    |> Task.WaitAll

    // Unsubscribe and stop the canceled event waiter
    meetingRequestReplyCanceled <! "Unsubscribe"

let ingestHistory system userId actorGroups =    

    // Send Complete command when we receive a Created event
    let roleConfirmationReaction = RolePlacementActors.spawnRoleConfirmationReaction system actorGroups

    // Process Create command, wait for Completed event
    let rolePlacementRequestReply =
        RequestReplyActor.spawnRequestReplyConditionalActor<RolePlacementCommand,RolePlacementEvent> 
            (fun cmd -> true)
            (fun evt -> evt.Item = RolePlacementEvent.Completed)
            system "rolePlacementRequestReply" actorGroups.RolePlacementActors

    // Cache of unprocessed placements for this script.
    let rolePlacementManager = RolePlacementActors.spawnPlacementManager system userId rolePlacementRequestReply

    let history = CsvFile.Load("C:\Users\Phillip Givens\OneDrive\Toastmasters\FilledRoles.csv").Cache()
    
    for row in history.Rows do
        printfn "History: (%s, %s, %s, %s)" 
            (row.GetColumn "Role") (row.GetColumn "Person") (row.GetColumn "Date") (row.GetColumn "Source")
            
    // Register activity related post action
    roleConfirmationReaction
    |> SubjectActor.subscribeTo actorGroups.RolePlacementActors.Events 

    // Get data
    history.Rows
    |> Seq.map (fun row -> ((row.GetColumn "Role"), (row.GetColumn "Person"), (row.GetColumn "Date"), (row.GetColumn "Source")))

    // Filter data
    |> Seq.where (fun (role, person, date, source) -> 
        role <> "" && person <> "" && date <> "" && source <> "")

    // Munge and enrich data
    |> Seq.map (fun (role, person, date, source) -> 
        let meeting = 
            date 
            |> System.DateTime.Parse 
            |> Persistence.ClubMeetings.findByDate 
        let roleTypeId = Persistence.RolePlacements.getRoleTypeId role
        let clubMember = Persistence.MemberManagement.findMemberByDisplayName person
        roleTypeId |> enum<RoleTypeId>, 
        meeting.Id |> MeetingId.box, 
        clubMember.Id |> MemberId.box , 
        RoleRequestId.Empty)    

    // Process data
    |> Seq.map (fun (roleTypeId, meetingId, clubMemberId, roleRequestId) ->
        (roleTypeId, meetingId, clubMemberId, roleRequestId)
        |> rolePlacementManager.Ask
        |> Async.AwaitTask)
        
    // Collect data
    |> Async.Parallel
    |> Async.Ignore
    |> Async.RunSynchronously

    // Unregister activity related post action
    roleConfirmationReaction
    |> SubjectActor.unsubscribeFrom actorGroups.RolePlacementActors.Events 

