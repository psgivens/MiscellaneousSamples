module ToastmastersRecord.Domain.Persistence.MemberManagement

open ToastmastersRecord.Data
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.MemberManagement
open ToastmastersRecord.Data.Entities

open Newtonsoft.Json
open System.Data.Entity

let persist (userId:UserId) (streamId:StreamId) (state:MemberManagementState option) =
    use context = new ToastmastersEFDbContext () 
    let entity = context.Members.Find (StreamId.unbox streamId)
    match entity, state with
    | null, Option.None -> ()
    | null, Option.Some(item) -> 
        context.Members.Add (
            MemberEntity (
                Id = StreamId.unbox streamId,
                IsActive = true,
                FullName = item.Details.Name,
                MemberId = TMMemberId.unbox item.Details.MemberId
            )) |> ignore
    | _, Option.None -> context.Members.Remove entity |> ignore        
    | _, Some(item) -> entity.FullName <- item.Details.Name
    context.SaveChanges () |> ignore
    
let find (userId:UserId) (streamId:StreamId) =
    use context = new ToastmastersEFDbContext () 
    context.Members.Find (StreamId.unbox streamId)


