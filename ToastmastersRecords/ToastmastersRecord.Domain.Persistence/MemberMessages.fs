module ToastmastersRecord.Domain.Persistence.MemberMessages

open ToastmastersRecord.Data
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.MemberMessages
open ToastmastersRecord.Data.Entities

open Newtonsoft.Json
open System.Data.Entity

let persist (userId:UserId) (streamId:StreamId) (state:Envelope<MemberMessageCommand> option) =
    use context = new ToastmastersEFDbContext () 
    let entity = context.Messages.Find (StreamId.unbox streamId)
    match entity, state with
    | null, Option.None -> ()
    | null, Option.Some(env) -> 
        match env.Item with 
        | Create(memberId, message) ->
            context.Messages.Add (
                MemberMessageEntity (
                    Id = StreamId.unbox streamId,
                    MemberId = MemberId.unbox memberId,
                    Message = message
                )) |> ignore
    | _, Option.None -> context.Messages.Remove entity |> ignore        
    | _, Some(item) -> ()
    context.SaveChanges () |> ignore
