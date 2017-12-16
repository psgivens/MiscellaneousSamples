module ToastmastersRecord.SampleApp.Schedule.MessageReview

open System
open ToastmastersRecord.Domain.Infrastructure
open ToastmastersRecord.Domain.DomainTypes
open ToastmastersRecord.Data.Entities
open ToastmastersRecord.Domain

let displayMessage userId (messageInfo:(MessageId * string * DateTime * string) * (MeetingId * DateTime) list * (string * ((MeetingId * DateTime) list)) list) = 
    let (mid, name, date, message), daysOff, requests = messageInfo
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
    |> Seq.iter (fun r ->
        r |> fst |> printfn "%s"
        r 
        |> snd 
        |> Seq.iter (fun days ->
            days
            |> snd
            |> fun d -> d.ToString "MMM dd, yyyy"
            |> printfn "    %s"))

    printfn """
Which dates would you like to work with? 
List all indices, comma delimitated.
-1 to end message.
"""
