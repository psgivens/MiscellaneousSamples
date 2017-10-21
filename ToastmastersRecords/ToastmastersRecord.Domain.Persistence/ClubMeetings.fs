﻿module ToastmastersRecord.Domain.Persistence.ClubMeetings

open ToastmastersRecord.Data
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.ClubMeetings
open ToastmastersRecord.Data.Entities

open Newtonsoft.Json
open System.Data.Entity

let persist (userId:UserId) (streamId:StreamId) (state:ClubMeetingState option) =
    use context = new ToastmastersEFDbContext () 
    let entity = context.ClubMeetings.Find (StreamId.unbox streamId)
    match entity, state with
    | null, Option.None -> ()
    | null, Option.Some(item) -> 
        context.ClubMeetings.Add (
            ClubMeetingEntity (
                Id = StreamId.unbox streamId,
                Date = item.Date,
                State = int item.State
            )) |> ignore
    | _, Option.None -> context.ClubMeetings.Remove entity |> ignore        
    | _, Some(item) -> entity.State <- int item.State
    context.SaveChanges () |> ignore
    
let find (userId:UserId) (streamId:StreamId) =
    use context = new ToastmastersEFDbContext () 
    context.Members.Find (StreamId.unbox streamId)

