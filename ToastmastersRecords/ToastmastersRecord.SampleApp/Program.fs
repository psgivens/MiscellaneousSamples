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

open ToastmastersRecord.Actors.Composition
open ToastmastersRecord.Domain.Persistence.ToastmastersEventStore

open System.Threading.Tasks

        

let spawnPlacementManager system userId (rolePlacmentRequestReply:IActorRef) =
    spawn system "rolePlacement_IngestManager" <| fun (mailbox:Actor<RoleTypeId * MeetingId * MemberId * RoleRequestId>) ->
        let placements =     
            use context = new ToastmastersRecord.Data.ToastmastersEFDbContext () 
            context.RolePlacements
            |> Seq.toList
        let rec loop (placements:ToastmastersRecord.Data.Entities.RolePlacementEntity list) = actor {
            let! roleTypeId, MeetingId.Id(meetingId), memberId, roleRequestId = mailbox.Receive ()
            let placement = placements |> List.find (fun p -> p.MeetingId = meetingId && p.RoleTypeId = int roleTypeId)

            (memberId, roleRequestId)
            |> RolePlacementCommand.Assign
            |> envelopWithDefaults
                (userId) 
                (TransId.create ()) 
                (StreamId.box placement.Id) 
                (Version.box 0s) 
            |> rolePlacmentRequestReply.Ask
            |> fun t -> mailbox.Sender () <! t.Result

            return! loop (placements |> List.where (fun p -> p <> placement))
        }        
        loop placements



open FSharp.Data
let ingestMembers system userId actorGroups =
    let memberRequestReply = spawnRequestReplyActor<MemberManagementCommand,MemberManagementEvent > system "memberManagement" actorGroups.MemberManagementActors
    // Download the stock prices
    let roster = CsvFile.Load("C:\Users\Phillip Givens\OneDrive\Toastmasters\Club-Roster20171002.csv").Cache()
    // Print the prices in the HLOC format
    roster.Rows 
    |> Seq.map (fun row ->
        printfn "Roster: (%s, %s, %s)" 
            (row.GetColumn "Customer ID") (row.GetColumn "Name") (row.GetColumn "Addr L1")

        let recordName = row.GetColumn "Name"
        let commaIndex = recordName.IndexOf ','
        let name, awards = 
            if commaIndex = -1 then recordName, ""
            else recordName.Substring (0, commaIndex), recordName.Substring commaIndex
        let toastmasterId = 
            row.GetColumn "Customer ID"
            |> System.Int32.Parse 
            |> TMMemberId.box;

        // Start by creating members        
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
    |> Seq.map (fun item -> 
        item
        |> MemberManagementCommand.Create
        |> envelopWithDefaults
            (userId)
            (TransId.create ())
            (StreamId.create ())
            (Version.box 0s)
        |> memberRequestReply.Ask)
    |> Seq.map (fun t -> t :> System.Threading.Tasks.Task)
    |> Seq.toArray
    |> System.Threading.Tasks.Task.WaitAll
    memberRequestReply <! "Unsubscribe"

let createMeetings system userId actorGroups =
    let meetingRequestReplyCreate = 
        spawnRequestReplyConditionalActor<ClubMeetingCommand,ClubMeetingEvent> 
            (fun x -> true)
            (fun x -> x.Item = Initialized)
            system "clubMeeting_initialized" actorGroups.ClubMeetingActors

    System.DateTime.Parse("07/11/2017")           
    |> Seq.unfold (fun d -> 
        if d < System.DateTime.Now then Some(d, d.AddDays 7.0)
        else None)
    |> Seq.map (fun d -> 
        // Create a meeting    
        d
        |> ClubMeetings.ClubMeetingCommand.Create
        |> envelopWithDefaults
            (userId)
            (TransId.create ())
            (StreamId.create ())
            (Version.box 0s)
        |> meetingRequestReplyCreate.Ask
        |> fun t -> t :> System.Threading.Tasks.Task)
    |> Seq.toArray
    |> System.Threading.Tasks.Task.WaitAll

    meetingRequestReplyCreate <! "Unsubscribe"

    let meetingRequestReplyCanceled = 
        spawnRequestReplyConditionalActor<ClubMeetingCommand,ClubMeetingEvent> 
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

    meetingRequestReplyCanceled <! "Unsubscribe"

let ingestHistory system userId actorGroups =
    (* *** Instructions for ingesting history ****
       [x] Create weekly meetings since the begining of the term (7/1)
       [x] Find and cancel weeks which had holidays or special events
       Subscribe trigger to automatically complete/confirm all roles
       [x] Read history from file
       Filter bad data (missing role, participant, etc.)       
       Foreach row
         - Lookup placement by Role and Date
         - Lookup user by name (will have to clean data by hand)
         - Assign user to placement
       Smart Proxy to wait for all role replacement logic to complete
       Subscribe trigger calculation of speech counts *)

    let history = CsvFile.Load("C:\Users\Phillip Givens\OneDrive\Toastmasters\FilledRoles.csv").Cache()
    // Print the prices in the HLOC format
    for row in history.Rows do
        printfn "History: (%s, %s, %s, %s)" 
            (row.GetColumn "Role") (row.GetColumn "Person") (row.GetColumn "Date") (row.GetColumn "Source")

    let confirmationActor = 
        (fun (mailbox:Actor<Envelope<RolePlacementEvent>>) cmdenv -> 
            match cmdenv.Item with
            // TODO: Respond to the event
            | _ -> ()) 
        |> actorOf2
        |> spawn system "RoleConfirmation" 

    confirmationActor
    |> SubjectActor.subscribeTo actorGroups.RolePlacementActors.Events 
        
    
    let rolePlacementRequestReply =
        spawnRequestReplyActor<RolePlacementCommand,RolePlacementEvent> 
            system "rolePlacementRequestReply" actorGroups.RolePlacementActors

    // RoleTypeId * MeetingId * MemberId * RoleRequestId
    let rolePlacementManager = spawnPlacementManager system userId rolePlacementRequestReply

    history.Rows
    |> Seq.map (fun row -> ((row.GetColumn "Role"), (row.GetColumn "Person"), (row.GetColumn "Date"), (row.GetColumn "Source")))
    |> Seq.where (fun (role, person, date, source) -> 
        role <> "" && person <> "" && date <> "" && source <> "")

    |> Seq.map (fun (role, person, date, source) -> 
        let meeting = 
            date 
            |> System.DateTime.Parse 
            |> Persistence.ClubMeetings.findByDate 
        let roleTypeId = Persistence.RolePlacements.getRoleTypeId role
        let clubMember = Persistence.MemberManagement.findMemberByDisplayName person
        roleTypeId, meeting.Id, clubMember.Id, RoleRequestId.Empty)    

    |> Seq.map (fun (roleTypeId, meetingId, clubMemberId, roleRequestId) ->
        (enum<RoleTypeId> roleTypeId, MeetingId.box meetingId, MemberId.box clubMemberId, roleRequestId)
        |> rolePlacementManager.Ask
        |> fun t -> t :> System.Threading.Tasks.Task)
        
    |> Seq.toArray
    |> System.Threading.Tasks.Task.WaitAll

    confirmationActor
    |> SubjectActor.unsubscribeFrom actorGroups.RolePlacementActors.Events 


let calculateHistory system userId actorGroups = ()

[<EntryPoint>]
let main argv =

    // System set up
    NewtonsoftHack.resolveNewtonsoft ()    
    let system = Configuration.defaultConfig () |> System.create "sample-system"
            
    let actorGroups = composeActors system
    
    // Sample data
    let userId = UserId.create ()
    
    actorGroups |> ingestMembers system userId
    actorGroups |> createMeetings system userId
    actorGroups |> ingestHistory system userId
    actorGroups |> calculateHistory system userId
       
    printfn "Press enter to continue"
    System.Console.ReadLine () |> ignore
    printfn "%A" argv
    0 // return an integer exit code
