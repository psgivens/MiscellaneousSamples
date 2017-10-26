// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

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

let onEvents sys name events =
    actorOf2 
    >> spawn sys (name + "_onEvents")
    >> SubjectActor.subscribeTo events

type ActorGroups = {
    MemberManagementActors:ActorIO
    MessageActors:ActorIO
    RoleRequestActors:ActorIO    
    RolePlacementActors:ActorIO
    ClubMeetingActors:ActorIO
    }

let composeActors system =
    // Create member management actors
    let memberManagementActors = 
        spawnEventSourcingActors 
            (system,
             "memberManagement", 
             MemberManagementEventStore (),
             buildState MemberManagement.evolve,
             MemberManagement.handle,
             Persistence.MemberManagement.persist)    

    let messageActors = 
        spawnCrudPersistActors 
            (system, 
             "memberMessage", 
             Persistence.MemberMessages.persist)

    // Create role request actors
    let roleRequestActors =
        spawnEventSourcingActors
            (system,
             "roleRequests",
             RoleRequestEventStore (),
             buildState RoleRequests.evolve,
             RoleRequests.handle,
             Persistence.RoleRequests.persist)

    // Create role request actors
    let rolePlacementActors =
        spawnEventSourcingActors
            (system,
             "rolePlacements",
             RolePlacementEventStore (),
             buildState RolePlacements.evolve,
             RolePlacements.handle,
             Persistence.RolePlacements.persist)   

    let placementRequestReplyCreate = 
        spawnRequestReplyConditionalActor<RolePlacementCommand,RolePlacementEvent> 
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
        spawnRequestReplyConditionalActor<RolePlacementCommand,RolePlacementEvent> 
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
        spawnEventSourcingActors 
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

let scriptInteractions roleRequesStreamId system actorGroups =
        onEvents system "onMemberCreated_createMemberMessage" actorGroups.MemberManagementActors.Events 
        <| fun (mailbox:Actor<Envelope<MemberManagementEvent>>) cmdenv ->
            printfn "onMemberCreated_createMemberMessage"             
            match cmdenv.Item with
            | MemberManagementEvent.Created (details) ->
                ((cmdenv.StreamId, "Here is a sample message from our member.")
                |> MemberMessageCommand.Create
                |> envelopWithDefaults
                    cmdenv.UserId
                    cmdenv.TransactionId
                    (StreamId.create ())
                    (Version.box 0s))
                |> actorGroups.MessageActors.In.Tell
            | _ -> ()
    
        // Wire up the Role Request actors    
        onEvents system "onMessageCreated_createRolerequest" actorGroups.MessageActors.Events
        <| fun (mailbox:Actor<Envelope<MemberMessageCommand>>) cmdenv ->        
            printfn "onMessageCreated_createRolerequest"
            match cmdenv.Item with
            | MemberMessageCommand.Create (mbrid, message) ->
                actorGroups.RoleRequestActors.In <!
                ((mbrid, cmdenv.StreamId,"S,TM",[])
                    |> RoleRequestCommand.Request
                    |> envelopWithDefaults
                        cmdenv.UserId
                        cmdenv.TransactionId
                        roleRequesStreamId
                        (Version.box 0s))
       
let debugger system name errorActor =
    let p mailbox cmdenv =
        System.Diagnostics.Debugger.Break ()
    actorOf2 p         
    |> spawn system name
    |> SubjectActor.subscribeTo errorActor
        
let doig system actorGroups = 
    use signal = new System.Threading.AutoResetEvent false

    // Sample data
    let userId = UserId.create ()
    let memberId = TMMemberId.box 456123
    let memberStreamId = StreamId.create ()
    let roleRequesStreamId = StreamId.create ()
    let meetingStreamId = StreamId.create ()

    actorGroups |> scriptInteractions roleRequesStreamId system         
    actorGroups.MessageActors.Errors |> debugger system "messageErrors"
    actorGroups.RoleRequestActors.Errors |> debugger system "requestErrors"

    // Set the wait event when the Role Request is created
    onEvents system "onRoleRequestCreated_signal" actorGroups.RoleRequestActors.Events
    <| fun (mailbox:Actor<Envelope<RoleRequestEvent>>) cmdenv ->        
        printfn "onRoleRequestCreated_signal"
        match cmdenv.Item with
        | Requested _ ->
            signal.Set () |> ignore
        | _ -> ()

    // Set the wait event when Clug Meeting is initialized
    onEvents system "onClubMeetingEvent_Signal" actorGroups.ClubMeetingActors.Events
    <| fun (mailbox:Actor<Envelope<ClubMeetings.ClubMeetingEvent>>) cmdenv ->        
            printfn "onClubMeetingEvent_Signal"
            match cmdenv.Item with
            | Initialized _ ->
                signal.Set () |> ignore
            | _ -> ()

    // Start by creating a member
    actorGroups.MemberManagementActors.In <!
        ({ MemberDetails.ToastmasterId = memberId;
           Name = "Phillip Scott Givens";             
           DisplayName = "Phillip Scott Givens";
           Awards="CC";
           Email="psgivens@gmail.com";
           HomePhone="949.394.2349";
           MobilePhone="949.394.2349";
           PaidUntil=System.DateTime.Now;
           ClubMemberSince=System.DateTime.Now;
           OriginalJoinDate=System.DateTime.Now;
           PaidStatus="paid";
           CurrentPosition="Vice President Education";
           SpeechCountConfirmedDate=System.DateTime.Now;
           }
         |> MemberManagementCommand.Create
         |> envelopWithDefaults
            (userId)
            (TransId.create ())
            (memberStreamId)
            (Version.box 0s))
   
    printfn "waiting on role request"
    signal.WaitOne -1 |> ignore
    printfn "role request created, done waiting"

    // [x] Query: Verify that the member exists
    let memberEntity = Persistence.MemberManagement.find userId memberStreamId
    if memberEntity = null then failwith "Member was not created"

    // [x] Query: Verify that a role request has been created 
    let requestEntity = Persistence.RoleRequests.find userId roleRequesStreamId
    if requestEntity = null then failwith "Role request was not created"

    // Create a meeting    
    actorGroups.ClubMeetingActors.In <!
        ((2017,10,24)
        |> System.DateTime
        |> ClubMeetings.ClubMeetingCommand.Create
        |> envelopWithDefaults
            (userId)
            (TransId.create ())
            (meetingStreamId)
            (Version.box 0s))
   
    printfn "waiting on club meeting"
    signal.WaitOne -1 |> ignore
    printfn "club meeting initialized, done waiting"

    // TODO: Query: Verify that role placements have been created
    let placements = Persistence.RolePlacements.findMeetingPlacements <| StreamId.unbox meetingStreamId
//    let meeting = Persistence.ClubMeetings.find userId meetingStreamId
    if placements.Length = 0 then failwith "Meeting created without role placements"
    // TODO: Command: Assign role as per request
    // TODO: Query: Verify that role request is marked assigned
    // TODO: Query: Verify that role placement is marked assigned

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

        // Start by creating a member
        
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
            }
        |> MemberManagementCommand.Create
        |> envelopWithDefaults
            (userId)
            (TransId.create ())
            (StreamId.create ())
            (Version.box 0s)
        |> memberRequestReply.Ask
        |> fun t -> t :> System.Threading.Tasks.Task)
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

    ["9/12/2017";"9/26/2017"]
    |> List.map System.DateTime.Parse
    |> List.map Persistence.ClubMeetings.findByDate
    |> Seq.map (fun meeting -> 
        ClubMeetings.ClubMeetingCommand.Cancel
        |> envelopWithDefaults
            (userId)
            (TransId.create ())
            (StreamId.box meeting.Id)
            (Version.box 0s)
        |> meetingRequestReplyCanceled.Ask
        |> fun t -> t :> Task)
    |> Seq.toArray
    |> Task.WaitAll

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


[<EntryPoint>]
let main argv =

    // System set up
    NewtonsoftHack.resolveNewtonsoft ()    
    let system = Configuration.defaultConfig () |> System.create "sample-system"
            
    let actorGroups = composeActors system

    // Sample data
    let userId = UserId.create ()
//    let memberId = TMMemberId.box 456123
//    let memberStreamId = StreamId.create ()
//    let roleRequesStreamId = StreamId.create ()
//    let meetingStreamId = StreamId.create ()
    
    actorGroups |> ingestMembers system userId
    actorGroups |> createMeetings system userId
    actorGroups |> ingestHistory system userId

       
    printfn "Press enter to continue"
    System.Console.ReadLine () |> ignore
    printfn "%A" argv
    0 // return an integer exit code
