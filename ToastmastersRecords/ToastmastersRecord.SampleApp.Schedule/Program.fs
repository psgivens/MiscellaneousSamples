// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System 
open ToastmastersRecord.Domain
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Data.Entities

let displayRequests userId (meeting:ClubMeetingEntity) =
    printfn ""
    printfn "#    Name                 Request"
    printfn "---- -------------------- ----------------"
    Persistence.MemberManagement.execQuery (fun context ->
        query { 
            for rrm in context.RoleRequestMeetings do
            join r in context.RoleRequests 
                on (rrm.RoleRequestId = r.Id)
            join m in context.Members 
                on (r.MemberId = m.Id)
            join h in context.MemberHistories
                on (r.MemberId = h.Id)
            where (rrm.MeetingId = meeting.Id && r.State = 0)
            select (m,h,r) })
    |> Seq.iteri (fun i (m,h,r) ->
        printfn "%-4d %-20s %s" i h.DisplayName r.Brief
        )
   

let displayMeeting userId (meeting:ClubMeetingEntity) (placements:RolePlacementEntity seq) =
    printfn ""
    printfn "Meeting: %s\t Status: %d"
        (meeting.Date.ToString ("MMM d, yyyy"))
        meeting.State
    printfn ""
    printfn "Item \tTMI Id   \tName                \tRole"
    printfn "---- \t---------\t--------------------\t---------------------"
    placements     
    |> Seq.iteri (fun i placement ->
        let roleType = placement.RoleTypeId |> enum<RoleTypeId>
        if placement.MemberId = Guid.Empty 
        then printfn "%-4d \t%-8d \t%-20s \t%-20s" i 0 "Not recorded" (roleType.ToString ())
        else
            let member' = Persistence.MemberManagement.find userId (placement.MemberId |> StreamId.box)
            let history = Persistence.MemberManagement.getMemberHistory member'.Id        
            printfn "%-4d \t%-9d\t%-20s\t%-20s" i member'.ToastmasterId history.DisplayName (roleType.ToString ()))

let displayMeetings (meetings:ClubMeetingEntity seq) =
    printfn ""
    printfn "Item \tDate         \tStatus    "
    printfn "---- \t------------ \t--------- "
    meetings 
    |> Seq.iteri (fun i meeting ->
        printfn "%-4d \t%-10s \t%d" i (meeting.Date.ToString "MMM dd, yyyy") meeting.State)

let processDate (f:DateTime -> unit) =
    printfn "Please enter a date"
    match Console.ReadLine () |> DateTime.TryParse with 
    | true, date -> f date
    | _ -> printfn "You entered an invalid date"

let rec editPlacement userId (placement:RolePlacementEntity) =
    let loop () = editPlacement userId placement
    let roleType = placement.RoleTypeId |> enum<RoleTypeId>
    let member' = Persistence.MemberManagement.find userId (placement.MemberId |> StreamId.box)
    let history = Persistence.MemberManagement.getMemberHistory member'.Id        
    printfn "Item to edit"
    printfn "Id: %d; Name:%s Role:%s" member'.ToastmasterId history.DisplayName (roleType.ToString ())
    let members = 
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
                    
    printfn ""
    printfn "#   TMI Id   Name                 Last TM    Last Facilitator "
    printfn "--- -------- -------------------- ---------- ---------------- "
    members 
    |> Seq.iteri (fun i (m,h,r) ->        
        printfn "%-3d %-8d %-20s %-10s %-10s %s" i m.ToastmasterId h.DisplayName (h.DateAsToastmaster.ToString "MM/dd/yyyy") (h.DateOfLastFacilitatorRole.ToString "MM/dd/yyyy") (if r <> null then r.Brief else "")
        )

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
        loop ()

let rec editMeeting userId (meeting:ClubMeetingEntity) (placements:RolePlacementEntity seq) =
    let loop = editMeeting userId meeting
    
    displayMeeting userId meeting placements    

    displayRequests userId meeting

    printfn """
type -1 for done editing or the index for the item you would like to edit. 
"""
    match Console.ReadLine () |> Int32.TryParse with
    | true, -1 -> ()
    | true, n when n < (placements |> Seq.length) -> 
        placements |> Seq.skip n |> Seq.head |> editPlacement userId 
        loop placements
    | _ -> 
        printfn "Input not understood"
        loop placements

let rec processCommands userId = 
    let loop () = processCommands userId
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
            editMeeting userId meeting placements)
        loop ()        
    | true, 4 -> 
        printfn "Create meeting not implemented"
        loop ()
    | true, i -> 
        printfn "Number not recognized: %d" i
        loop ()
    | _ -> 
        printfn "Not a number" 
        loop ()
        
[<EntryPoint>]
let main argv = 
    let userId = Persistence.Users.findUserId "ToastmastersRecord.SampleApp.Schedule" 
    processCommands userId
    printfn "%A" argv
    0 // return an integer exit code
