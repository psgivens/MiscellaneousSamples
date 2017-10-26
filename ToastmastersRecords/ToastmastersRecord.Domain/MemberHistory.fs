module ToastmastersRecord.Domain.MemberHistory


open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.RolePlacements 
open System

type MemberRoleHistory = { RoleTypeId:RoleTypeId; Date:DateTime }
type MemberRoleHistoryCommand =
    | Calculate

type MemberRoleHistoryState = { LastTM:DateTime; TotalSpeeches:int }

type MemberRoleHistoryEvent =
    | Calculating of MemberRoleHistoryState
    | DataAcquired of MemberRoleHistory list
    | Complete
    
let handle evtSeq (state:MemberRoleHistoryEvent option) (cmdenv:Envelope<MemberRoleHistoryCommand>) =
    let raise evt evtSeq  = 
        let raise', evtSeq' = (Seq.head evtSeq, Seq.tail evtSeq)
        raise' evt
        evtSeq' 

    match cmdenv.Item with 
    | Calculate -> 
        async {

            evtSeq 
            |> raise (MemberRoleHistoryEvent.Calculating 
                     { LastTM=DateTime.Now; TotalSpeeches=0 } )

            |> raise (MemberRoleHistoryEvent.DataAcquired [])

            |> raise (MemberRoleHistoryEvent.Complete) 

            |> ignore

        } |> Async.Start

