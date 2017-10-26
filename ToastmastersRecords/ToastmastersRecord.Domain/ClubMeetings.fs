module ToastmastersRecord.Domain.ClubMeetings

open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.CommandHandler

open System.Threading.Tasks

type ClubMeetingCommand =
    | Create of System.DateTime
    | Cancel
    | Occur

type ClubMeetingEvent =
    | Created of System.DateTime
    | Initialized
    | Canceling
    | Canceled
    | Occurred

type ClubMeetingStateValue =
    | Initializing = 0
    | Pending = 10
    | Canceling = 20
    | Canceled = 30
    | Occurred = 40

type ClubMeetingState = { State:ClubMeetingStateValue; Date:System.DateTime }

let (|MatchStateValue|_|) state =
    match state with 
    | Some(value) -> Some(value.State, value)
    | _ -> None 

type RoleActions = { 
    createRole: Envelope<ClubMeetingCommand> -> RoleTypeId -> Task<obj>
    cancelRoles: Envelope<ClubMeetingCommand> -> Task
    }

let handleProto 
        (roleActions:RoleActions) 
        (command:CommandHandlers<ClubMeetingEvent>)
        (state:ClubMeetingState option) 
        (cmdenv:Envelope<ClubMeetingCommand>) 
            : CommandHandlerFunction<ClubMeetingEvent>=

    match cmdenv.Item with 
    | ClubMeetingCommand.Create date  ->
        command.block {
            do! ClubMeetingEvent.Created date |> raise 
            return async {
                do! [1..12] 
                    |> List.map (fun i -> 
                        enum<RoleTypeId> i 
                        |> roleActions.createRole cmdenv
                        :> Task) 
                    |> List.toArray
                    |> Task.WhenAll
                    |> Async.AwaitTask

                return ClubMeetingEvent.Initialized    
            }
        }

    | _ -> command.event ClubMeetingEvent.Occurred 

let handle (roleActions:RoleActions) raiseFunctions (state:ClubMeetingState option) (cmdenv:Envelope<ClubMeetingCommand>) =
    let raiseOnce = Seq.head raiseFunctions >> ignore

    let createMeeting date =
        async {
            let raise, raiseFunctions = (Seq.head raiseFunctions, Seq.tail raiseFunctions)
            ClubMeetingEvent.Created date |> raise 

            do! [1..12] 
                |> List.map (fun i -> 
                    enum<RoleTypeId> i 
                    |> roleActions.createRole cmdenv
                    :> Task) 
                |> List.toArray
                |> Task.WhenAll
                |> Async.AwaitTask

            let raise, raiseFunctions = (Seq.head raiseFunctions, Seq.tail raiseFunctions)
            ClubMeetingEvent.Initialized |> raise
        } 
        |> Async.Start

    let cancelMeeting () = 
        async {
            let raise, raiseFunctions = (Seq.head raiseFunctions, Seq.tail raiseFunctions)
            ClubMeetingEvent.Canceling |> raise 

            do! roleActions.cancelRoles cmdenv 
                |> Async.AwaitTask 

            let raise, raiseFunctions = (Seq.head raiseFunctions, Seq.tail raiseFunctions)
            ClubMeetingEvent.Canceled |> raise
        } 
        |> Async.Start

    match state, cmdenv.Item with
    | None, ClubMeetingCommand.Create date -> createMeeting date
    | MatchStateValue (ClubMeetingStateValue.Pending, _), ClubMeetingCommand.Cancel -> cancelMeeting ()
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
    | MatchStateValue (ClubMeetingStateValue.Pending, s), ClubMeetingEvent.Canceling -> { s with State=ClubMeetingStateValue.Canceling }
    | MatchStateValue (ClubMeetingStateValue.Canceling, s), ClubMeetingEvent.Canceled -> { s with State=ClubMeetingStateValue.Canceled }
    | MatchStateValue (ClubMeetingStateValue.Pending, s), ClubMeetingEvent.Occurred -> { s with State=ClubMeetingStateValue.Occurred }
    | None, _ -> failwith "A meeting must first be created to cancel or occur"
    | Some _, ClubMeetingEvent.Created _ -> failwith "cannot create a meeting which already exists"
    | MatchStateValue (ClubMeetingStateValue.Occurred, _), _ -> failwith "Occurred is an ending state"
    | MatchStateValue (ClubMeetingStateValue.Canceled, _), _ -> failwith "Canceled is an ending state"
    | _, _ -> failwith "Unexpected state/command combination"
