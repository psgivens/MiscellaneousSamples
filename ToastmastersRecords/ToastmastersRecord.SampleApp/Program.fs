// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open ToastmastersRecord.SampleApp.Initialize
open ToastmastersRecord.SampleApp.Infrastructure

open System
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
    
    let roster = CsvFile.Load("C:\Users\Phillip Givens\OneDrive\Toastmasters\Club-Roster20171118.csv").Cache()
    
    // Map CSV rows to Member Details
    roster.Rows 
    |> Seq.map (fun row ->
        let recordName = row.GetColumn "Name"
        let commaIndex = recordName.IndexOf ','
        let name, awards = 
            if commaIndex = -1 then recordName, ""
            else recordName.Substring (0, commaIndex), commaIndex + 2 |> recordName.Substring 
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

let ingestSpeechCount system userId (actorGroups:ActorGroups) = 
    let roster = CsvFile.Load("C:\Users\Phillip Givens\OneDrive\Toastmasters\ConfirmedSpeechCount.csv").Cache()
    
    roster.Rows 
    |> Seq.iter (fun row ->
        printfn "Count: (%s, %s, %s)" 
            (row.GetColumn "Name") (row.GetColumn "Count") (row.GetColumn "Date"))

    let historyRequestReplyCanceled = 
        RequestReplyActor.spawnRequestReplyActor<MemberHistoryConfirmation,unit> 
            system "history_countconfirmed" actorGroups.MemberHistoryActors

    roster.Rows    
    |> Seq.map (fun row -> 
        let refDate : System.DateTime ref = ref DateTime.MinValue
        let defaultDate = "1900/1/1" |> System.DateTime.Parse
        let objNext = if DateTime.TryParse (row.GetColumn "ObjNext", refDate)
                        then !refDate
                        else defaultDate
        let date = if DateTime.TryParse (row.GetColumn "Date", refDate)
                        then !refDate
                        else defaultDate
        let intRef : int ref = ref 0
        let count = if System.Int32.TryParse(row.GetColumn "Count", intRef) then !intRef else 0
        row.GetColumn "Customer ID" |> System.Int32.Parse, 
        row.GetColumn "Name", 
        row.GetColumn "Display Name",
        objNext,
        count,
        date)
    |> Seq.map (fun (tmid, name, displayName, objNext, count, date) ->        
        let clubMember = Persistence.MemberManagement.findMemberByToastmasterId tmid
        
        clubMember.Id |> StreamId.box,
        {   MemberHistoryConfirmation.SpeechCount = count
            MemberHistoryConfirmation.ConfirmationDate = date
            MemberHistoryConfirmation.DisplayName = displayName
             })
    |> Seq.map (fun (id, confirmation) ->
        confirmation
        |> envelopWithDefaults
            userId
            (TransId.create ())
            id
            (Version.box 0s)
        |> historyRequestReplyCanceled.Ask
        |> Async.AwaitTask)

    // Wait for completion of all meeting creations
    |> Async.Parallel
    |> Async.Ignore
    |> Async.RunSynchronously
         
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
    let roleConfirmationReaction = RolePlacementActors.spawnRoleConfirmationReaction system actorGroups
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

let confirmPlacements system userId actorGroups = 
    Persistence.MemberManagement.getMemberHistories ()
    |> Seq.iter (fun (_, history) ->        
        history.Id 
        |> Persistence.RolePlacements.getRolePlacmentsByMember
        |> Seq.fold (fun state (date,placement) -> 
            match placement.RoleTypeId |> enum<RoleTypeId> with
            | RoleTypeId.Speaker ->           
                if date <= history.SpeechCountConfirmedDate then state
                else { state with 
                        MemberHistoryState.SpeechCount = state.SpeechCount + 1 
                        MemberHistoryState.LastSpeechGiven = date }
            | RoleTypeId.Toastmaster ->       { state with MemberHistoryState.LastToastmaster = date }
            | RoleTypeId.TableTopicsMaster -> { state with MemberHistoryState.LastTableTopicsMaster = date }
            | RoleTypeId.GeneralEvaluator  -> { state with MemberHistoryState.LastGeneralEvaluator = date }
            | RoleTypeId.Evaluator         -> { state with MemberHistoryState.LastEvaluationGiven = date }
            | _ -> state
            ) {
                MemberHistoryState.SpeechCount = history.ConfirmedSpeechCount
                MemberHistoryState.LastToastmaster = history.DateAsToastmaster
                MemberHistoryState.LastTableTopicsMaster = history.DateAsTableTopicsMaster
                MemberHistoryState.LastGeneralEvaluator = history.DateAsGeneralEvaluator
                MemberHistoryState.LastSpeechGiven = history.DateOfLastSpeech
                MemberHistoryState.LastEvaluationGiven = history.DateOfLastEvaluation
            }
        |> Persistence.MemberManagement.persistHistory userId (StreamId.box history.Id))

let calculateHistory system userId actorGroups = 
    Persistence.MemberManagement.getMemberHistories ()
    |> Seq.iter (fun (_, history) ->        
        history.Id 
        |> Persistence.RolePlacements.getRolePlacmentsByMember
        |> Seq.fold (fun state (date,placement) -> 
            match placement.RoleTypeId |> enum<RoleTypeId> with
            | RoleTypeId.Speaker ->           
                if date <= history.SpeechCountConfirmedDate then state
                else { state with 
                        MemberHistoryState.SpeechCount = state.SpeechCount + 1 
                        MemberHistoryState.LastSpeechGiven = date }
            | RoleTypeId.Toastmaster ->       { state with MemberHistoryState.LastToastmaster = date }
            | RoleTypeId.TableTopicsMaster -> { state with MemberHistoryState.LastTableTopicsMaster = date }
            | RoleTypeId.GeneralEvaluator  -> { state with MemberHistoryState.LastGeneralEvaluator = date }
            | RoleTypeId.Evaluator         -> { state with MemberHistoryState.LastEvaluationGiven = date }
            | _ -> state
            ) {
                MemberHistoryState.SpeechCount = history.ConfirmedSpeechCount
                MemberHistoryState.LastToastmaster = history.DateAsToastmaster
                MemberHistoryState.LastTableTopicsMaster = history.DateAsTableTopicsMaster
                MemberHistoryState.LastGeneralEvaluator = history.DateAsGeneralEvaluator
                MemberHistoryState.LastSpeechGiven = history.DateOfLastSpeech
                MemberHistoryState.LastEvaluationGiven = history.DateOfLastEvaluation
            }
        |> Persistence.MemberManagement.persistHistory userId (StreamId.box history.Id))

let defaultDate = "1900/1/1" |> System.DateTime.Parse
let interpret (date:System.DateTime) =
    if date = defaultDate then "Data not available" else date.ToString "MM/dd/yyyy"
let generateMessages system userId actorGroups =
    use writer = new System.IO.StreamWriter "C:\Users\Phillip Givens\OneDrive\Toastmasters\messages.txt"
    Persistence.MemberManagement.getMemberHistories ()
    |> Seq.where (fun (m,h) -> m.Awards |> System.String.IsNullOrWhiteSpace |> not &&
                               h.ConfirmedSpeechCount = 0)
    |> Seq.iter (fun (m, h) ->
        sprintf """
-------------------------
%s

Hello %s,

I would like to finish this term strong with accurate data about
our members and where they are in their journey. Please let me 
know if you know how many speeches you've accomplished toward 
your next award or when you held a major role. 

Toastmaster ID: %d
Awards: %s
Speeches toward next award: %s
Last Toastmaster: %s
Last Table Topics Master Role: %s
Last General Evaluator Spot Held: %s
Last Speech Given: %s

Please also let me know if you plan on continuing toward the 
old award system or if you are going to start fresh with 
Pathways. 

Thanks,
Phillip Scott Givens, CC
Vice President of Education
Toastmasters - Santa Monica Club 21
            """ 
                m.Email
                h.DisplayName 
                m.ToastmasterId 
                (if m.Awards |> System.String.IsNullOrWhiteSpace 
                    then "None" 
                    else m.Awards)
                (h.CalculatedSpeechCount.ToString ())
                (interpret h.DateAsToastmaster)
                (interpret h.DateAsTableTopicsMaster)
                (interpret h.DateAsGeneralEvaluator)
                (interpret h.DateOfLastSpeech)
        |> writer.Write
        ())

(* Creating a CSV file *)
// prepare a string for writing to CSV  
let prepareStr obj =
    obj.ToString()
     .Replace("\"","\"\"") // replace single with double quotes
     |> sprintf "\"%s\""   // surround with quotes

// convert a list of strings to a CSV
let listToCsv list =
    let combine s1 s2 = s1 + "," + s2
    list 
    |> Seq.map prepareStr 
    |> Seq.reduce combine 

type MemHistCsvType = 
    CsvProvider<
        Schema = "Name (string), Last Speech (string), Last Evaluation (string), Last TM (string), Last TTM (string), Last GE (string)", 
        HasHeaders=false>

let generateStatistics system userId actorGroups =    
    let fileName = "C:\Users\Phillip Givens\OneDrive\Toastmasters\HistoryStats3.csv"
    let histories = Persistence.MemberManagement.getMemberHistories ()
    let csv = 
        new MemHistCsvType
            ((("Name", "Last Speech", "Last Evaluation", "Last TM", "Last TTM", "Last GE") |> MemHistCsvType.Row)
            :: 
            (histories
             |> List.map (fun (m,h) -> 
                h.DisplayName, 
                interpret h.DateOfLastSpeech,
                interpret h.DateOfLastEvaluation,
                interpret h.DateAsToastmaster,
                interpret h.DateAsTableTopicsMaster,
                interpret h.DateAsGeneralEvaluator)
             |> Seq.map (fun x -> x |> MemHistCsvType.Row)
             |> Seq.toList))
    csv.Save fileName

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
    actorGroups |> generateMessages system userId
    actorGroups |> generateStatistics system userId

    printfn "Press enter to continue"
    System.Console.ReadLine () |> ignore
    printfn "%A" argv
    0 // return an integer exit code
