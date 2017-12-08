// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System 
open ToastmastersRecord.Domain
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Data.Entities


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

let rec editMeeting userId (meeting:ClubMeetingEntity) (placements:RolePlacementEntity seq) =
    let loop = editMeeting userId meeting
    
    displayMeeting userId meeting placements    
    printfn """
Which item would you like to edit?
0) Done editing
1) ...
"""
    match Console.ReadLine () |> Int32.TryParse with
    | true, 0 -> ()
    | _ -> 
        printfn "Input not understood"
        loop placements

let rec processCommands userId = 
    let loop () = processCommands userId
    printfn """
Please make a selection
0) exit
2) List 5 meetings from date
1) List meeting details
3) Edit meeting details
4) ...
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
