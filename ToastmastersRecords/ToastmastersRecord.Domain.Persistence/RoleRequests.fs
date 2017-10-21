module ToastmastersRecord.Domain.Persistence.RoleRequests

open ToastmastersRecord.Data
open ToastmastersRecord.Data.Entities
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.RoleRequests

open Newtonsoft.Json
open System.Data.Entity

let persist (userId:UserId) (streamId:StreamId) (state:RoleRequestState option) =
    use context = new ToastmastersEFDbContext () 
    let entity = context.RoleRequests.Find (StreamId.unbox streamId)
    match entity, state with
    | null, Option.None -> ()
    | null, Option.Some(item) ->              
        let dates =
            item.Dates
            |> List.map (fun d -> 
                RoleRequestDate (
                    RoleRequestId = StreamId.unbox streamId,
                    Date = d))
            |> System.Collections.Generic.List 
        context.RoleRequests.Add (
            RoleRequestEntity (
                Id = StreamId.unbox streamId,
                State = int item.State, 
                MessageId = MessageId.unbox item.MessageId,
                Brief = item.Brief,
                MemberId = MemberId.unbox item.MemberId,
                Dates = dates
            )) |> ignore
    | _, Option.None -> context.RoleRequests.Remove entity |> ignore        
    | _, Some(item) -> 
        () // TODO: update
    context.SaveChanges () |> ignore
    
let find (userId:UserId) (streamId:StreamId) =
    use context = new ToastmastersEFDbContext () 
    context.RoleRequests.Find (StreamId.unbox streamId)


