﻿module ToastmastersRecord.Domain.RoleRequests

open System
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.CommandHandler

type RoleRequestCommand = 
    | Request of MemberId * MessageId * RequestAbbreviation * DateTime list
    | Assign
    | Unassign
    | Complete
    | Cancel

type RoleRequestEvent = 
    | Requested of MemberId * MessageId * RequestAbbreviation * DateTime list
    | Assigned
    | Unassigned
    | Completed
    | Canceled

type RoleRequestStateValue = 
    | Unassigned = 0
    | Assigned = 1
    | Complete = 2
    | Canceled = 3
type RoleRequestState = { State:RoleRequestStateValue; MemberId:MemberId; MessageId:MessageId; Brief:RequestAbbreviation; Dates:DateTime list }

let (|HasStateValue|_|) expected state =
    match state with 
    | Some(value) when value.State = expected -> Some value
    | _ -> None 
        
let handle (command:CommandHandlers<RoleRequestEvent, Version>) (state:RoleRequestState option) (cmdenv:Envelope<RoleRequestCommand>) = 
    match state, cmdenv.Item with
    | None, Request(mbrid, msgid, rab, dtl) -> RoleRequestEvent.Requested(mbrid,msgid, rab, dtl)
    | HasStateValue RoleRequestStateValue.Unassigned _, RoleRequestCommand.Assign -> RoleRequestEvent.Assigned
    | HasStateValue RoleRequestStateValue.Unassigned _, RoleRequestCommand.Cancel -> RoleRequestEvent.Canceled
    | HasStateValue RoleRequestStateValue.Assigned _, RoleRequestCommand.Unassign -> RoleRequestEvent.Unassigned
    | HasStateValue RoleRequestStateValue.Assigned _, RoleRequestCommand.Complete -> RoleRequestEvent.Completed    
    | _, RoleRequestCommand.Complete -> failwith "Request must be assigned to be completed"
    | _, RoleRequestCommand.Unassign -> failwith "Request must be assigned to be unassigned"
    | _, RoleRequestCommand.Assign -> failwith "Request must be unassigned to be assigned"
    | _, RoleRequestCommand.Cancel -> failwith "Request must be unassigned to be canceled"
    | Some(_), Request(_) -> failwith "Cannot make a request on existing request"
    |> command.event

let evolve (state:RoleRequestState option) (event:RoleRequestEvent) : RoleRequestState= 
    match state, event with
    | None, RoleRequestEvent.Requested(mbrid, msgid, rab, dtl) ->
            { RoleRequestState.State=RoleRequestStateValue.Unassigned; MemberId=mbrid; MessageId=msgid; Brief=rab; Dates=dtl }
    | HasStateValue RoleRequestStateValue.Unassigned s, RoleRequestEvent.Assigned -> { s with State=RoleRequestStateValue.Assigned }
    | HasStateValue RoleRequestStateValue.Unassigned s, RoleRequestEvent.Canceled -> { s with State=RoleRequestStateValue.Canceled }
    | HasStateValue RoleRequestStateValue.Assigned s, RoleRequestEvent.Unassigned -> { s with State=RoleRequestStateValue.Unassigned }
    | HasStateValue RoleRequestStateValue.Assigned s, RoleRequestEvent.Completed ->  { s with State= RoleRequestStateValue.Complete }    
    | _, RoleRequestEvent.Completed -> failwith "Request must be assigned to be completed"
    | _, RoleRequestEvent.Unassigned -> failwith "Request must be assigned to be unassigned"
    | _, RoleRequestEvent.Assigned -> failwith "Request must be unassigned to be assigned"
    | _, RoleRequestEvent.Canceled -> failwith "Request must be unassigned to be canceled"
    | Some(_), RoleRequestEvent.Requested(_) -> failwith "Cannot make a request on existing request"

