module ToastmastersRecord.Domain.ClubMeetings

open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open System.Threading.Tasks

type ClubMeetingCommand =
    | Create of System.DateTime
    | Cancel
    | Occur

type ClubMeetingEvent =
    | Created of System.DateTime
    | Initialized
    | Canceled
    | Occurred

type ClubMeetingStateValue =
    | Initializing = 0
    | Pending = 1
    | Canceled = 2
    | Occurred = 3

type ClubMeetingState = { State:ClubMeetingStateValue; Date:System.DateTime }

let (|MatchStateValue|_|) state =
    match state with 
    | Some(value) -> Some(value.State, value)
    | _ -> None 

let handle (createRole: Envelope<ClubMeetingCommand> -> RoleTypeId -> Task<obj>) raiseFunctions (state:ClubMeetingState option) (cmdenv:Envelope<ClubMeetingCommand>) =
    let raiseOnce = Seq.head raiseFunctions >> ignore
    match state, cmdenv.Item with
    | None, ClubMeetingCommand.Create date -> 
        async {
            let raise, raiseFunctions = (Seq.head raiseFunctions, Seq.tail raiseFunctions)
            ClubMeetingEvent.Created date |> raise 

            do! [1..13] 
                |> List.map (fun i -> 
                    enum<RoleTypeId> i 
                    |> createRole cmdenv
                    :> Task) 
                |> List.toArray
                |> Task.WhenAll
                |> Async.AwaitTask

            let raise, raiseFunctions = (Seq.head raiseFunctions, Seq.tail raiseFunctions)
            ClubMeetingEvent.Initialized |> raise
        } 
        |> Async.Start
    | MatchStateValue (ClubMeetingStateValue.Pending, _), ClubMeetingCommand.Cancel -> raiseOnce <| ClubMeetingEvent.Canceled
    | MatchStateValue (ClubMeetingStateValue.Pending, _), ClubMeetingCommand.Occur -> raiseOnce <| ClubMeetingEvent.Occurred
    | None, _ -> failwith "A meeting must first be created to cancel or occur"
    | Some _, ClubMeetingCommand.Create _ -> failwith "cannot create a meeting which already exists"
    | MatchStateValue (ClubMeetingStateValue.Occurred, _), _ -> failwith "Occurred is an ending state"
    | MatchStateValue (ClubMeetingStateValue.Canceled, _), _ -> failwith "Canceled is an ending state"
    | _, _ -> failwith "Unexpected state/command combination"

let evolve (state:ClubMeetingState option) (event:ClubMeetingEvent) =
    match state, event with
    | None, ClubMeetingEvent.Created date -> { State=ClubMeetingStateValue.Initializing; Date=date }
    | MatchStateValue (ClubMeetingStateValue.Initializing, s), ClubMeetingEvent.Initialized -> { s with State=ClubMeetingStateValue.Pending }
    | MatchStateValue (ClubMeetingStateValue.Pending, s), ClubMeetingEvent.Canceled -> { s with State=ClubMeetingStateValue.Canceled }
    | MatchStateValue (ClubMeetingStateValue.Pending, s), ClubMeetingEvent.Occurred -> { s with State=ClubMeetingStateValue.Occurred }
    | None, _ -> failwith "A meeting must first be created to cancel or occur"
    | Some _, ClubMeetingEvent.Created _ -> failwith "cannot create a meeting which already exists"
    | MatchStateValue (ClubMeetingStateValue.Occurred, _), _ -> failwith "Occurred is an ending state"
    | MatchStateValue (ClubMeetingStateValue.Canceled, _), _ -> failwith "Canceled is an ending state"
    | _, _ -> failwith "Unexpected state/command combination"
