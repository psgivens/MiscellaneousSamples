﻿// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System 
open Akka.Actor
open Akka.FSharp

open ToastmastersRecord.Domain
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Data.Entities
open ToastmastersRecord.SampleApp.Schedule.PrintMeetings
open ToastmastersRecord.SampleApp.Schedule.EditMeeting

open ToastmastersRecord.Actors
open ToastmastersRecord.Domain.RolePlacements
open ToastmastersRecord.Domain.ClubMeetings
open ToastmastersRecord.SampleApp.Initialize

let processCommands system (actorGroups:ActorGroups) userId = 
    // Process Create command, wait for Completed event
    let rolePlacementRequestReply =
        RequestReplyActor.spawnRequestReplyActor<RolePlacementCommand,RolePlacementEvent> 
            system "rolePlacementRequestReply" actorGroups.RolePlacementActors

    // Create an request-reply actor that we can wait on. 
    let meetingRequestReplyCreate = 
        RequestReplyActor.spawnRequestReplyConditionalActor<ClubMeetingCommand,ClubMeetingEvent> 
            (fun x -> true)
            (fun x -> x.Item = Initialized)
            system "clubMeeting_initialized" actorGroups.ClubMeetingActors

    let rec loop () = 
        printfn """
Please make a selection
0) exit
1) List 5 meetings from date
2) List meeting details
3) Edit meeting details
4) Create new meeting
5) ...
    """
        match Console.ReadLine () |> Int32.TryParse with
        | true, 0 -> 
            printfn "Exiting"
            ()
        | true, 1 -> 
            processDate (Persistence.ClubMeetings.fetchByDate 5 >> displayMeetings)
            loop ()
        | true, 2 ->
            processDate (fun date ->          
                let meeting = Persistence.ClubMeetings.findByDate date
                let placements = Persistence.RolePlacements.findMeetingPlacements meeting.Id  
                displayMeeting userId meeting placements)
            loop ()        
        | true, 3 ->
            processDate (fun date ->          
                let meeting = Persistence.ClubMeetings.findByDate date
                let placements = Persistence.RolePlacements.findMeetingPlacements meeting.Id  
                editMeeting rolePlacementRequestReply userId meeting placements)
            loop ()        
        | true, 4 -> 
            printfn "Enter the date of the meeeting."
            match Console.ReadLine () |> DateTime.TryParse with
            | true, date -> 
                date
                |> ClubMeetings.ClubMeetingCommand.Create
                |> envelopWithDefaults
                    (userId)
                    (TransId.create ())
                    (StreamId.create ())
                    (Version.box 0s)
                |> meetingRequestReplyCreate.Ask
                |> Async.AwaitTask
                |> Async.Ignore
                |> Async.RunSynchronously
                let meeting = Persistence.ClubMeetings.findByDate date
                let placements = Persistence.RolePlacements.findMeetingPlacements meeting.Id
                displayMeeting userId meeting placements 
            | _ -> printfn "Input could not be parsed as a date."            
            loop ()
        | true, i -> 
            printfn "Number not recognized: %d" i
            loop ()
        | _ -> 
            printfn "Not a number" 
            loop ()
    loop ()    

[<EntryPoint>]
let main argv = 

    // System set up
    NewtonsoftHack.resolveNewtonsoft ()    
    let system = Configuration.defaultConfig () |> System.create "sample-system"
            
    let actorGroups = composeActors system

    let userId = Persistence.Users.findUserId "ToastmastersRecord.SampleApp.Schedule" 
    processCommands system actorGroups userId
    printfn "%A" argv
    0 // return an integer exit code
