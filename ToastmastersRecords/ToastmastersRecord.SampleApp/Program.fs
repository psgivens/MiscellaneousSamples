// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open ToastmastersRecord.SampleApp.Initialize
open ToastmastersRecord.SampleApp.Infrastructure

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

open FSharp.Data
let ingestMembers system userId actorGroups =
    let memberRequestReply = RequestReplyActor.spawnRequestReplyActor<MemberManagementCommand,MemberManagementEvent> system "memberManagement" actorGroups.MemberManagementActors
    
    let roster = CsvFile.Load("C:\Users\Phillip Givens\OneDrive\Toastmasters\Club-Roster20171002.csv").Cache()
    
    // Map CSV rows to Member Details
    roster.Rows 
    |> Seq.map (fun row ->
        let recordName = row.GetColumn "Name"
        let commaIndex = recordName.IndexOf ','
        let name, awards = 
            if commaIndex = -1 then recordName, ""
            else recordName.Substring (0, commaIndex), recordName.Substring commaIndex
        let toastmasterId = 
            row.GetColumn "Customer ID"
            |> System.Int32.Parse 
            |> TMMemberId.box;

        {   MemberDetails.ToastmasterId = toastmasterId
            Name = name
            DisplayName = name
            Awards = awards
            Email= row.GetColumn "Email";
            HomePhone = row.GetColumn "Home Phone";
            MobilePhone= row.GetColumn "Mobile Phone";               
            PaidUntil = row.GetColumn "Paid Until" |> System.DateTime.Parse;
            ClubMemberSince = row.GetColumn "Member of Club Since" |> System.DateTime.Parse;
            OriginalJoinDate = row.GetColumn "Original Join Date" |> System.DateTime.Parse;
            SpeechCountConfirmedDate = row.GetColumn "Original Join Date" |> System.DateTime.Parse;
            PaidStatus = row.GetColumn "status (*)";
            CurrentPosition = row.GetColumn "Current Position";
            })

    // Use the member details to send envelope with command to actor and wait for reply
    |> Seq.map (fun memberDetails -> 
        async {
            printfn "Send: (%s, %s, %d)" 
                memberDetails.DisplayName
                memberDetails.Awards
                (TMMemberId.unbox memberDetails.ToastmasterId)

            do! memberDetails
                |> MemberManagementCommand.Create
                |> envelopWithDefaults
                    (userId)
                    (TransId.create ())
                    (StreamId.create ())
                    (Version.box 0s)
                |> memberRequestReply.Ask
                |> Async.AwaitTask
                |> Async.Ignore

            printfn "Created: (%s, %s, %d)" 
                memberDetails.DisplayName
                memberDetails.Awards
                (TMMemberId.unbox memberDetails.ToastmasterId)
        })
    |> Async.Parallel
    |> Async.Ignore
    |> Async.RunSynchronously

    // Unsubscribe and stop the actor
    memberRequestReply <! "Unsubscribe"

let ingestSpeechCount  system userId actorGroups = 
    let roster = CsvFile.Load("C:\Users\Phillip Givens\OneDrive\Toastmasters\ConfirmedSpeechCount.csv").Cache()
    
    roster.Rows 
    |> Seq.iter (fun row ->
        printfn "Count: (%s, %s, %s)" 
            (row.GetColumn "Name") (row.GetColumn "Count") (row.GetColumn "Date"))

    roster.Rows
    |> Seq.map (fun row -> 
        row.GetColumn "Name", 
        row.GetColumn "Count" |> System.Int32.Parse, 
        row.GetColumn "Date" |> System.DateTime.Parse)
    |> Seq.map (fun (name, count, date) ->        
        let clubMember = Persistence.MemberManagement.findMemberByDisplayName name
        
        clubMember.Id |> StreamId.box,
        {   MemberHistoryConfirmation.SpeechCount = count
            MemberHistoryConfirmation.ConfirmationDate = date })
    |> Seq.iter (fun (id, confirmation) ->

        // TODO: Create and post to a CRUD persistence actor
        Persistence.MemberManagement.persistConfirmation userId id confirmation)


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
            (fun x -> x.Item = Canceled)
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

    let roleConfirmationReaction = RolePlacementActors.spawnRoleConfirmationReaction system
    let rolePlacementRequestReply =
        RequestReplyActor.spawnRequestReplyActor<RolePlacementCommand,RolePlacementEvent> 
            system "rolePlacementRequestReply" actorGroups.RolePlacementActors
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

let calculateHistory system userId actorGroups = 
    Persistence.MemberManagement.getMemberHistories ()
    |> Seq.iter (fun history ->        
        history.Id 
        |> Persistence.RolePlacements.getRolePlacementsByMemberSinceDate history.SpeechCountConfirmedDate
        |> Seq.fold (fun state (date,placement) -> 
            match placement.RoleTypeId |> enum<RoleTypeId> with
            | RoleTypeId.Speaker ->           { state with MemberHistoryState.SpeechCount = state.SpeechCount + 1 }
            | RoleTypeId.Toastmaster ->       { state with MemberHistoryState.LastToastmaster = date }
            | RoleTypeId.TableTopicsMaster -> { state with MemberHistoryState.LastTableTopicsMaster = date }
            | RoleTypeId.GeneralEvaluator  -> { state with MemberHistoryState.LastGeneralEvaluator = date }
            | _ -> state
            ) {
                MemberHistoryState.SpeechCount = history.ConfirmedSpeechCount
                MemberHistoryState.LastToastmaster = history.DateAsToastmaster
                MemberHistoryState.LastTableTopicsMaster = history.DateAsTableTopicsMaster
                MemberHistoryState.LastGeneralEvaluator = history.DateAsGeneralEvaluator
            }
        |> Persistence.MemberManagement.persistHistory userId (StreamId.box history.Id))

[<EntryPoint>]
let main argv =

    // System set up
    NewtonsoftHack.resolveNewtonsoft ()    
    let system = Configuration.defaultConfig () |> System.create "sample-system"
            
    let actorGroups = composeActors system
    
    // Sample data
    let userId = UserId.create ()
    
    actorGroups |> ingestMembers system userId
    actorGroups |> ingestSpeechCount system userId
    actorGroups |> createMeetings system userId
    actorGroups |> ingestHistory system userId
    actorGroups |> calculateHistory system userId
       
    printfn "Press enter to continue"
    System.Console.ReadLine () |> ignore
    printfn "%A" argv
    0 // return an integer exit code
