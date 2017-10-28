﻿module ToastmastersRecord.Domain.Persistence.RolePlacements

open ToastmastersRecord.Data
open ToastmastersRecord.Data.Entities
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Domain.RolePlacements

open Newtonsoft.Json
open System.Data.Entity
open System

let persist (userId:UserId) (streamId:StreamId) (state:RolePlacementState option) =
    use context = new ToastmastersEFDbContext () 
    let entity = context.RolePlacements.Find (StreamId.unbox streamId)
    match entity, state with
    | null, Option.None -> ()
    | null, Option.Some(item) ->                     
        let state, memberId, roleRequestId = 
            match item.State with
            | Assigned (mid, rrid) -> 1, MemberId.unbox mid, RoleRequestId.unbox rrid
            | Complete (mid, rrid) -> 2, MemberId.unbox mid, RoleRequestId.unbox rrid
            | _ -> 0, System.Guid.Empty, System.Guid.Empty

        context.RolePlacements.Add (
            RolePlacementEntity (
                Id = StreamId.unbox streamId,
                State = state, 
                MemberId = memberId,
                RoleRequestId = roleRequestId,
                RoleTypeId = int item.RoleTypeId,
                MeetingId = MeetingId.unbox item.MeetingId
            )) |> ignore
    | _, Option.None -> context.RolePlacements.Remove entity |> ignore        
    | _, Some(item) -> 
        () // TODO: update
    context.SaveChanges () |> ignore
    
let find (userId:UserId) (streamId:StreamId) =
    use context = new ToastmastersEFDbContext () 
    context.RolePlacements.Find (StreamId.unbox streamId)

let findMeetingPlacements id = 
    use context = new ToastmastersEFDbContext () 
    query { for placement in context.RolePlacements do
            where (placement.MeetingId = id)
            select placement}
    |> Seq.toList

let getPlacmentByMeetingAndRole roleTypeId meetingId = 
    use context = new ToastmastersEFDbContext () 
    query { for placement in context.RolePlacements do
            where (placement.RoleTypeId = roleTypeId 
            && placement.MeetingId = meetingId)
            select placement
            exactlyOne }
            
let getRoleTypeId name =
    use context = new ToastmastersEFDbContext ()
    query { for roleType in context.RoleTypes do 
            where (roleType.Title = name)
            select roleType.Id 
            exactlyOne }
                