

//            ________________
//    divisor ) dividend
//
//   dividend / divisor = quotient

let cut key acc =
    let rec cut' pre post =
        match post with
        | [] -> pre |> List.rev,[]
        | item::tail ->
            if item = key then tail, item::pre |> List.rev
            else tail |> cut' (item::pre) 
    cut' [] acc

let foldWhile sequence =
    let rec foldWhile' pre post = 
        let quotient,remainder as key = post |> Seq.head
        if remainder = 0 then (key::pre |> List.rev,[])
        else match pre |> cut key with
                | _,[] -> post |> Seq.tail |> foldWhile' (key::pre) 
                | pre',post' -> (pre' |> List.rev,post')
    foldWhile' [] sequence

let division dividend divisor = 
    Seq.unfold (fun dividend -> 
        let dividend' = dividend * 10
        let quotient = dividend' / divisor
        let remainder = dividend' % divisor
        Some((quotient,remainder), remainder)
    ) dividend

open System.Text
let printQuotient (once,repeat) =
    let builder = StringBuilder("0.")
    once 
    |> List.map fst
    |> List.iter (fun (item:int) -> item |> builder.Append |> ignore)
    match repeat with
    | [] -> ()
    | _ -> 
        builder.Append '(' |> ignore
        repeat
        |> List.map fst
        |> List.iter (fun (item:int) -> item |> builder.Append |> ignore)    
        builder.Append ')' |> ignore
    builder.ToString ()


division 1 8 
|> foldWhile
|> printQuotient
|> printfn "%s"

division 2 7 
|> foldWhile
|> printQuotient
|> printfn "%s"

division 23 56 
|> foldWhile
|> printQuotient
|> printfn "%s"




let cutBy2 key values =
    let rec cut' pre = function
        | [] -> pre, []
        | head::post -> 
            if head = key then post |> List.rev, head::pre 
            else post |> cut' (head::pre)
    cut' [] values

let foldWhile2 sequence =
    let rec foldWhile' acc values =
        let (quotient,remainder) as key = values |> Seq.head
        if remainder = 0 then key::acc |> List.rev, []
        else match acc |> cutBy2 key with 
                | _, [] -> values |> Seq.tail |> foldWhile' (key::acc)
                | validSplit -> validSplit
    sequence |> foldWhile' [] 

let division2 dividend divisor = 
    Seq.unfold (fun dividend' -> 
        let dividend'' = dividend' * 10
        let quotient = dividend'' / divisor
        let remainder = dividend'' % divisor
        Some((quotient, remainder), remainder)) dividend



division2 1 8 |> foldWhile2 |> printQuotient
division2 2 7 |> foldWhile2 |> printQuotient
division2 23 56 |> foldWhile2 |> printQuotient


