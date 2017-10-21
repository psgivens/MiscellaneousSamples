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

let onEvents sys name events =
    actorOf2 
    >> spawn sys name
    >> SubjectActor.subscribeTo events


[<EntryPoint>]
let main argv = 

    use signal = new System.Threading.AutoResetEvent false

    // System set up
    NewtonsoftHack.resolveNewtonsoft ()    
    let system = Configuration.defaultConfig () |> System.create "sample-system"
        
    // Sample data
    let userId = UserId.create ()
    let memberId = TMMemberId.box 456123
    let memberStreamId = StreamId.create ()
    let roleRequesStreamId = StreamId.create ()
    //let makeHandle handle f state cmdenv = handle f state cmdenv.Item
        
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

    let debugger name errorActor =
        let p mailbox cmdenv =
            System.Diagnostics.Debugger.Break ()
        actorOf2 p         
        |> spawn system name
        |> SubjectActor.subscribeTo errorActor
        
    messageActors.Errors |> debugger "messageErrors"
    roleRequestActors.Errors |> debugger "requestErrors"

    onEvents system "onMemberCreated_createMemberMessage" memberManagementActors.Events 
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
            |> messageActors.In.Tell
        | _ -> ()
    
    // Wire up the Role Request actors    
    onEvents system "onMessageCreated_createRolerequest" messageActors.Events
    <| fun (mailbox:Actor<Envelope<MemberMessageCommand>>) cmdenv ->        
        printfn "onMessageCreated_createRolerequest"
        match cmdenv.Item with
        | MemberMessageCommand.Create (mbrid, message) ->
            roleRequestActors.In <!
            ((mbrid, cmdenv.StreamId,"S,TM",[])
                |> RoleRequestCommand.Request
                |> envelopWithDefaults
                    cmdenv.UserId
                    cmdenv.TransactionId
                    roleRequesStreamId
                    (Version.box 0s))
       
    // Wire up the Role Request actors    
    onEvents system "onRoleRequestCreated_signal" roleRequestActors.Events
    <| fun (mailbox:Actor<Envelope<RoleRequestEvent>>) cmdenv ->        
        printfn "onRoleRequestCreated_signal"
        match cmdenv.Item with
        | Requested _ ->
            signal.Set () |> ignore
        | _ -> ()
       
    
    // Start by creating a member
    // [x] Command: Create a Member
    memberManagementActors.In <!
        ({ MemberDetails.Name = "Phillip Scott Givens"; MemberId = memberId }
         |> MemberManagementCommand.Create
         |> envelopWithDefaults
            (userId)
            (TransId.create ())
            (memberStreamId)
            (Version.box 0s))
   
    printfn "waiting on role request"
    signal.WaitOne -1 |> ignore
    printfn "role request created, done waiting"
    //System.Console.ReadLine () |> ignore
    
    // TODO: Wait for actor to complete

    // [x] Query: Verify that the member exists
    let memberEntity = Persistence.MemberManagement.find userId memberStreamId
    if memberEntity = null then failwith "Member was not created"
    
    // [x] Command: Create role request
    // [x] Query: Verify that a role request has been created 
    let requestEntity = Persistence.RoleRequests.find userId roleRequesStreamId
    if requestEntity = null then failwith "Role request was not created"


    // Create role request actors
    let rolePlacementActors =
        spawnEventSourcingActors
            (system,
             "rolePlacements",
             RolePlacementEventStore (),
             buildState RolePlacements.evolve,
             RolePlacements.handle,
             Persistence.RolePlacements.persist)
    

    let placementRequestReply = spawnRequestReplyActor<RolePlacementCommand,RolePlacementEvent> system "rolePlacement" rolePlacementActors
    let createRolePlacement meetingEnv roleTypeId = 
        ((roleTypeId, MeetingId.box <| StreamId.unbox meetingEnv.StreamId)
        |> RolePlacementCommand.Open
        |> envelopWithDefaults
            (userId)
            (meetingEnv.TransactionId)
            (StreamId.create ())
            (Version.box 0s))
        |> placementRequestReply.Ask
        
    // Create member management actors
    let clubMeetingActors = 
        spawnEventSourcingActors 
            (system,
             "clubMeetings", 
             ClubMeetingEventStore (),
             buildState ClubMeetings.evolve,
             (ClubMeetings.handle createRolePlacement),
             Persistence.ClubMeetings.persist)    

    onEvents system "onClubMeetingEvent_Signal" clubMeetingActors.Events
    <| fun (mailbox:Actor<Envelope<ClubMeetings.ClubMeetingEvent>>) cmdenv ->        
            printfn "onClubMeetingEvent_Signal"
            match cmdenv.Item with
            | Initialized _ ->
                signal.Set () |> ignore
            | _ -> ()
    
    clubMeetingActors.In <!
        ((2017,10,24)
        |> System.DateTime
        |> ClubMeetings.ClubMeetingCommand.Create
        |> envelopWithDefaults
            (userId)
            (TransId.create ())
            (memberStreamId)
            (Version.box 0s))
   
    printfn "waiting on club meeting"
    signal.WaitOne -1 |> ignore
    printfn "club meeting initialized, done waiting"

    // TODO: [ ] Command: Create Meeting
    // TODO: Query: Verify that role placements have been created

    // TODO: Command: Assign role as per request
    // TODO: Query: Verify that role request is marked assigned
    // TODO: Query: Verify that role placement is marked assigned

    printfn "%A" argv
    0 // return an integer exit code
