// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System

open Akka.Actor
open Akka.FSharp
open FSharp.Data

open ToastmastersRecord.Actors
open ToastmastersRecord.Domain
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.ClubMeetings
open ToastmastersRecord.SampleApp.Infrastructure
open ToastmastersRecord.SampleApp.Initialize
open ToastmastersRecord.SampleApp.IngestMembers
open ToastmastersRecord.SampleApp.IngestMeetings


let defaultDate = "1900/1/1" |> DateTime.Parse
let interpret (date:DateTime) =
    if date = defaultDate then "Data not available" else date.ToString "MM/dd/yyyy"

type MeetingsCsvType = 
    CsvProvider<
        Schema = "Meeting Id (string), Meeting Date (string), Concluded (string)", 
        Separators = "\t",
        HasHeaders=false>

type MessagesCsvType = 
    CsvProvider<
        Schema = "Message Id (string), Name (string), Date (string), Message (string)", 
        Separators = "\t",
        HasHeaders=false>

type DayOffCsvType =
    CsvProvider<
        Schema = "Meeting Id (string), Message Id (string), Name (string)", 
        HasHeaders=false>

type RoleRequestCsvType =
    CsvProvider<
        Schema = "Meeting Id (string), Message Id (string), Name (string), Description (string)", 
        HasHeaders=false>

let generateMeetings system userId actorGroups =    
    let fileName = "C:\Users\Phillip Givens\OneDrive\Toastmasters\ClubMeetings.csv"
    
//    let csvFile = new MeetingsCsvType ([("Meeting Id", "Meeting Date", "Concluded")|> MeetingsCsvType.Row])
    let csvFile = new MeetingsCsvType ([])
    let csvFile' = 
        DateTime.Parse("07/11/2017")           
        |> Seq.unfold (fun date -> 
            if date < DateTime.Now then Some(date, date.AddDays 7.0)
            else None)
        |> Seq.map (fun date -> 
            ((Guid.NewGuid ()).ToString "D", interpret date, "true"))
        |> Seq.map MeetingsCsvType.Row
        |> csvFile.Append

    let endOfYear = DateTime.Parse("12/31/2017")
    let csvFile'' = 
        DateTime.Parse("11/28/2017")           
        |> Seq.unfold (fun date -> 
            if date < endOfYear then Some(date, date.AddDays 7.0)
            else None)
        |> Seq.map (fun date -> 
            ((Guid.NewGuid ()).ToString "D", interpret date, "false"))
        |> Seq.map MeetingsCsvType.Row
        |> csvFile'.Append

    csvFile''.Save fileName

let addIdToMemberMessages system userId (actorGroups:ActorGroups) = 
    let messages = 
        CsvFile.Load(
            "C:\Users\Phillip Givens\OneDrive\Toastmasters\RoleRequestMessages.txt",
            separators="\t",
            hasHeaders=true).Cache()
    let fileName = "C:\Users\Phillip Givens\OneDrive\Toastmasters\RoleRequestMessagesId.txt"

//    let csvFile = new MessagesCsvType  ([("Meeting Id", "Name", "Date", "Message")|> MessagesCsvType.Row])
    let csvFile = new MessagesCsvType  ([])

    let csv = 
        messages.Rows 
        |> Seq.map (fun row ->
            ((Guid.NewGuid ()).ToString "D"), (row.GetColumn "Name"), (row.GetColumn "Date"), (row.GetColumn "Message"))
        |> Seq.map MessagesCsvType .Row
        |> csvFile.Append
    csv.Save fileName

type Meeting = {
    Id:MeetingId
    Date:DateTime
    Concluded:bool
    }

type Description = string
type Request =
    | Unavailable of MessageId
    | Available of Description * MessageId list

type RequestInfo = {
    MessageId:MessageId
    Name:string
    Request:Request
    }
    

let buildRequests () = 
    let meetingsFile = MeetingsCsvType.Load "C:\Users\Phillip Givens\OneDrive\Toastmasters\ClubMeetings.csv" 
    let messagesFile = MessagesCsvType.Load "C:\Users\Phillip Givens\OneDrive\Toastmasters\RoleRequestMessagesId.txt"

    let absentsFileName = "C:\Users\Phillip Givens\OneDrive\Toastmasters\RequestOff.csv"
    let requestFileName = "C:\Users\Phillip Givens\OneDrive\Toastmasters\RequestOn.csv"

    IO.File.WriteAllText (absentsFileName, String.Empty)
    IO.File.WriteAllText (requestFileName, String.Empty)

    use absentsFile = new DayOffCsvType      ([("Meeting Id", "Message Id", "Name")|> DayOffCsvType.Row])
    use requestFile = new RoleRequestCsvType ([("Meeting Id", "Message Id", "Name", "Description")|> RoleRequestCsvType.Row])

    let unscheduledMeetings = 
        meetingsFile.Rows
        |> Seq.map (fun row -> 
            {   Meeting.Id = row.``Meeting Id`` |> Guid.Parse |> MeetingId.box
                Date = row.``Meeting Date`` |> DateTime.Parse
                Concluded = row.Concluded |> Boolean.Parse } )
        |> Seq.where (fun meeting -> not meeting.Concluded)
        |> Seq.toArray

    messagesFile.Rows
    |> Seq.fold (fun (state:RequestInfo list) row -> 
        Seq.unfold (fun unscheduledMeetings ->
            if unscheduledMeetings |> Array.isEmpty then None
            else

                printfn """
Which dates would you like to work with? 
List all indices, comma delimitated.
-1 to end message.
"""
                unscheduledMeetings
                |> Array.iteri (fun i meeting ->
                    printfn "%d) %s" i (meeting.Date.ToString "MMM dd, yyyy"))

                let response = Console.ReadLine ()
                if response = "-1" then None
                else
                    let selected =
                        response.Split ','
                        |> Seq.map (fun r ->
                            let i = r.Trim () |> Int32.Parse
                            unscheduledMeetings.[i]
                            )
                        |> Seq.toList                    
                        
                    printfn """
Would you like to 
1) Request the day off
2) Describe your request
"""
                    match Console.ReadLine () |> Int32.Parse with
                    | 1 -> 
                        selected
                        |> List.map (fun meeting ->
                            {   RequestInfo.MessageId = row.``Message Id`` |> Guid.Parse |> MessageId.box
                                RequestInfo.Name = row.Name
                                RequestInfo.Request = meeting.Id |> Request.Unavailable})
                        |> fun l -> 
                            Some(l, 
                                unscheduledMeetings 
                                |> Array.filter (fun meeting -> 
                                    selected 
                                    |> List.contains meeting 
                                    |> not))
                    | 2 -> 
                        printfn "Please describe the request"
                        ([{ RequestInfo.MessageId = row.``Message Id`` |> Guid.Parse |> MessageId.box
                            RequestInfo.Name = row.Name
                            RequestInfo.Request = 
                                (Console.ReadLine (), selected |> List.map (fun meeting -> meeting.Id))
                                |> Request.Available }],
                         unscheduledMeetings)
                        |> Some
                    | _ -> 
                        printfn "You've entered an invalid value"
                        Some ([], unscheduledMeetings)

            ) unscheduledMeetings
        |> Seq.fold (fun allItems rowItems -> rowItems@allItems) state
        ) []
    |> Seq.fold (fun ((absents:Runtime.CsvFile<DayOffCsvType.Row>), (requests:Runtime.CsvFile<RoleRequestCsvType.Row>)) request -> 
        match request.Request with
        | Unavailable (MeetingId.Id meetingId) -> 
            [(meetingId.ToString "D", request.MessageId |> MessageId.toString, request.Name)] 
            |> Seq.map DayOffCsvType.Row
            |> absents.Append, 
            requests
        | Available (description, meetingIds) -> 
            absents,
            [(String.Join (";", meetingIds |> List.map (fun (MeetingId.Id i) -> i.ToString "D")), 
              request.MessageId |> MessageId.toString, 
              request.Name, 
              description)] 
            |> Seq.map RoleRequestCsvType.Row
            |> requests.Append            
        ) (absentsFile :> Runtime.CsvFile<DayOffCsvType.Row>, 
           requestFile :> Runtime.CsvFile<RoleRequestCsvType.Row>) 
    |> fun (absents, requests) ->
        absents.Save absentsFileName
        requests.Save requestFileName

[<EntryPoint>]
let main argv = 
    // System set up
    NewtonsoftHack.resolveNewtonsoft ()    
    let system = Configuration.defaultConfig () |> System.create "sample-system"
            
    let actorGroups = composeActors system
    
    // Sample data
    let userId = UserId.create ()
    
    actorGroups |> generateMeetings system userId
    actorGroups |> addIdToMemberMessages system userId
    // actorGroups |> ingestMembers system userId
    //actorGroups |> ingestMemberMessages system userId 

    buildRequests ()
    
    printfn "%A" argv
    0 // return an integer exit code
