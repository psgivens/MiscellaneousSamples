// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open ToastmastersRecord.SampleApp
open ToastmastersRecord.SampleApp.Initialize
open ToastmastersRecord.SampleApp.Infrastructure

open System
open Akka.Actor
open Akka.FSharp
open FSharp.Data

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

open ToastmastersRecord.SampleApp.IngestMembers
open ToastmastersRecord.SampleApp.IngestMeetings
         
let ingestMemberMessages system userId (actorGroups:ActorGroups) = 
    let roster = CsvFile.Load("C:\Users\Phillip Givens\OneDrive\Toastmasters\RoleRequestMessages.txt","\t").Cache()

    let messageRequestReply = 
        RequestReplyActor.spawnRequestReplyActor<MemberMessageCommand,unit> 
            system "memberMessage_ingest" actorGroups.MessageActors
    
    roster.Rows 
    |> Seq.iter (fun row ->
        printfn "Count: (%s, %s, %s)" 
            (row.GetColumn "Name") (row.GetColumn "Date") (row.GetColumn "Message"))

    roster.Rows 
    |> Seq.map (fun row ->
       (row.GetColumn "Name"), (row.GetColumn "Date"), (row.GetColumn "Message"))
    // TODO: Convert rows
    // TODO: Send rows to message actors
    |> ignore

    failwith "ingestMemberMessages not implemented."

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
        |> Seq.where (fun (d, p) -> p.State = 2)
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
    let csvFile = new MemHistCsvType ([("Name", "Last Speech", "Last Evaluation", "Last TM", "Last TTM", "Last GE") |> MemHistCsvType.Row])

    let csv = 
        histories
        |> List.map (fun (m,h) -> 
            h.DisplayName, 
            interpret h.DateOfLastSpeech,
            interpret h.DateOfLastEvaluation,
            interpret h.DateAsToastmaster,
            interpret h.DateAsTableTopicsMaster,
            interpret h.DateAsGeneralEvaluator)
        |> Seq.map MemHistCsvType.Row
        |> csvFile.Append
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
    actorGroups |> ingestMemberMessages system userId 

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
