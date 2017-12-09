module ToastmastersRecord.SampleApp.Schedule.EditMeeting

open System 
open Akka.Actor
open Akka.FSharp

open ToastmastersRecord.Domain
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Data.Entities
open ToastmastersRecord.SampleApp.Schedule.PrintMeetings

open ToastmastersRecord.Actors
open ToastmastersRecord.Domain.RolePlacements
open ToastmastersRecord.SampleApp.Initialize


let getMembers (placement:RolePlacementEntity) = 
    let roleType = placement.RoleTypeId |> enum<RoleTypeId>
    Persistence.MemberManagement.execQuery (fun context ->
        let attending = 
            query { for clubMember in context.Members do   
                    join history in context.MemberHistories
                        on (clubMember.Id = history.Id)
                    where (clubMember.PaidStatus = "Paid")
                
                    // Where there is no day off record
                    leftOuterJoin dayOff in 
                        query { for d in context.DaysOff do
                                where (d.MeetingId = placement.MeetingId)
                                select d } 
                        on (clubMember.Id = dayOff.MemberId) into result
                    for d in result do
                    where (d = null)                

                    // Where the member is not already assigned a role
                    leftOuterJoin assigned in 
                        query { for p in context.RolePlacements do
                                where (p.MeetingId = placement.MeetingId)
                                select p } 
                        on (clubMember.Id = assigned.MemberId) into result
                    for p in result do
                    where (p = null)                

                    // Pull in any requests the member may have made
                    leftOuterJoin request in 
                        query { for rrm in context.RoleRequestMeetings do
                                join r in context.RoleRequests 
                                    on (rrm.RoleRequestId = r.Id)
                                where (rrm.MeetingId = placement.MeetingId && r.State = 0)
                                select r } 
                        on (clubMember.Id = request.MemberId) into result
                    for request in result do
                    select (clubMember, history, request) }            

        match roleType with
        | RoleTypeId.Toastmaster -> 
            query { for clubMember, history, request in attending do 
                    where (history.EligibilityCount >= 5)
                    sortBy history.DateAsToastmaster
                    thenBy history.DateOfLastFacilitatorRole
                    select (clubMember, history, request)
                    take 100 }

        | RoleTypeId.GeneralEvaluator -> 
            query { for clubMember, history, request in attending do 
                    where (history.EligibilityCount >= 4)
                    sortBy history.DateAsGeneralEvaluator
                    thenBy history.DateOfLastFacilitatorRole
                    select (clubMember, history, request)
                    take 100 }

        | RoleTypeId.TableTopicsMaster -> 
            query { for clubMember, history, request in attending do 
                    where (history.EligibilityCount >= 3)
                    sortBy history.DateAsTableTopicsMaster
                    thenBy history.DateOfLastFacilitatorRole
                    select (clubMember, history, request )
                    take 100 }

        | RoleTypeId.Evaluator -> 
            query { for clubMember, history, request  in attending do 
                    where (history.EligibilityCount >= 3)
                    sortBy history.DateOfLastEvaluation
                    thenBy history.DateOfLastMajorRole
                    select (clubMember, history, request )
                    take 100 }

        | RoleTypeId.JokeMaster
        | RoleTypeId.ClosingThoughtAndGreeter
        | RoleTypeId.OpeningThoughtAndBallotCounter -> 
            query { for clubMember, history, request  in attending do 
                    sortBy history.DateOfLastMinorRole
                    select (clubMember, history, request )
                    take 100 }

        | RoleTypeId.ErAhCounter
        | RoleTypeId.Grammarian
        | RoleTypeId.Videographer
        | RoleTypeId.Timer -> 
            query { for clubMember, history, request  in attending do 
                    sortBy history.DateOfLastFunctionaryRole
                    select (clubMember, history, request )
                    take 100 }                

        | _ -> attending
    )
    
let displayMembers roleType (members:(MemberEntity * MemberHistoryAggregate * RoleRequestEntity) seq) = 
    printfn ""
    printfn "#   TMI Id   Name                 Last TM    Last Facilitator "
    printfn "--- -------- -------------------- ---------- ---------------- "
    members 
    |> Seq.iteri (fun i (m,h,r) ->        
        printfn "%-3d %-8d %-20s %-10s %-10s %s" i m.ToastmasterId h.DisplayName (h.DateAsToastmaster.ToString "MM/dd/yyyy") (h.DateOfLastFacilitatorRole.ToString "MM/dd/yyyy") (if r <> null then r.Brief else "")
        )

let rec editPlacement system (actorGroups:ActorGroups) userId (placement:RolePlacementEntity) =
        // Process Create command, wait for Completed event
    let rolePlacementRequestReply =
        RequestReplyActor.spawnRequestReplyConditionalActor<RolePlacementCommand,RolePlacementEvent> 
            (fun cmd -> true)
            (fun evt -> match evt.Item with | RolePlacementEvent.Assigned(_,_) -> true; | _ -> false)
            //(fun evt -> evt.Item = RolePlacementEvent.Completed)
            system "rolePlacementRequestReply" actorGroups.RolePlacementActors

    let rec loop (placement:RolePlacementEntity) = 
        let roleType = placement.RoleTypeId |> enum<RoleTypeId>
        let member' = Persistence.MemberManagement.find userId (placement.MemberId |> StreamId.box)
        let history = Persistence.MemberManagement.getMemberHistory member'.Id        
        printfn "Item to edit"
        printfn "Id: %d; Name:%s Role:%s" member'.ToastmasterId history.DisplayName (roleType.ToString ())
        let members = getMembers placement
        members |> displayMembers roleType

        if placement.MemberId <> Guid.Empty then
            printfn "The role of %s is currently assigned to %s. Would you like to unassign?" (roleType.ToString ()) history.DisplayName
            printfn "0) No    1) yes"
            match Console.ReadLine () |> Int32.TryParse with
            | true, 0 -> ()
            | true, 1 -> 
                RolePlacementCommand.Unassign
                |> envelopWithDefaults
                    userId
                    (TransId.create ())
                    (placement.Id |> StreamId.box)
                    (Version.box 0s)
                |> rolePlacementRequestReply.Tell
            | _ -> 
                printfn "Received an invalid answser"
                loop placement
        else
            printfn "Which member would you like? Enter -1 for no change."
            match Console.ReadLine () |> Int32.TryParse with
            | true, n when n < 0 -> ()
            | true, n when n < members.Length -> 
                let m,h,r = members |> Seq.skip n |> Seq.head
                printfn "TODO: You chose: %s" h.DisplayName
            | true, n -> 
                printfn "The number entered was out of range"
            | _ -> 
                printfn "You entered invalid input"
                loop placement
    loop placement

let rec editMeeting system (actorGroups:ActorGroups) userId (meeting:ClubMeetingEntity) (placements:RolePlacementEntity seq) =
    let loop = editMeeting system actorGroups userId meeting
    
    displayMeeting userId meeting placements    

    displayRequests userId meeting

    printfn """
type -1 for done editing or the index for the item you would like to edit. 
"""
    match Console.ReadLine () |> Int32.TryParse with
    | true, -1 -> ()
    | true, n when n < (placements |> Seq.length) -> 
        placements |> Seq.skip n |> Seq.head |> editPlacement system actorGroups userId 
        loop placements
    | _ -> 
        printfn "Input not understood"
        loop placements

