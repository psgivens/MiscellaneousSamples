module ToastmastersRecord.SampleApp.Schedule.MessageReview

open System
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Data.Entities
open ToastmastersRecord.Domain

let displayMessage userId (messageInfo:(MessageId * MemberId * string * DateTime * string) * (MeetingId * DateTime) list * (RoleRequestId * string * int * ((MeetingId * DateTime) list)) list) = 
    let (msgId, memId, name, date, message), daysOff, requests = messageInfo
    printfn """
%s said:
%s
----
"""
        name
        message

    // Days off
    printfn "Days off"
    printfn "----------"
    daysOff 
    |> Seq.iter (fun dayOff -> 
        dayOff 
        |> snd
        |> fun d -> d.ToString "MMM dd, yyyy"
        |> printfn "%s")

    // Special requests
    printfn "----------"
    printfn "Instructions"
    printfn "----------"
    requests 
    |> Seq.iter (fun (id, description, state, r) ->
        printfn "`%s` has state %d" description state
        r 
        |> Seq.iter (fun days ->
            days
            |> snd
            |> fun d -> d.ToString "MMM dd, yyyy"
            |> printfn "    %s"))

    printfn "#####################################################"
