open System.Text
let printQuotient dividend divisor =
    let cutBy pair = 
        let rec cut' prev = function
            | [] -> (prev |> List.rev,[])
            | head::tail when head = pair -> (tail, head::prev)
            | head::tail -> cut' (head::prev) tail
        cut' []

    let foldWhile input =
        let rec foldWhile' acc input' = 
            let (quotient,remainder) as pair = input' |> Seq.head
            if remainder = 0 then (pair::acc |> List.rev,[])
            else 
                match acc |> cutBy pair with
                | _,[] -> input' |> Seq.tail |> foldWhile' (pair::acc)
                | pre,post -> pre |> List.rev,post
        foldWhile' [] input
        
    //  2/4 [((0,0),20);((2,0),0)]
    //  2/5 [(0,0);(4,0)]
    //  2/7 [(0,0);(2,6);(8,);(5,)]
    Seq.unfold (function
            | 0 -> None
            | state -> 
                let dividend = state * 10
                let quotient = dividend / divisor
                let remainder = dividend % divisor                
                Some ((quotient,remainder), remainder)
        ) dividend 
    |> foldWhile 
    |> (fun (nr,repeating) ->
        let append (items:(int*_) list) builder = 
            items 
            |> List.fold (fun (builder':StringBuilder) item ->
                item |> fst |> builder'.Append) builder

        StringBuilder "0."
        |> append nr
        |> fun builder -> 
            match repeating with 
            | [] -> builder
            | values -> 
                builder.Append '(' 
                |> append values 
                |> ignore
                builder.Append ')'
        |> fun builder -> printfn "%s" (builder.ToString())
        )     

printQuotient 23 56
printQuotient 2 7
printQuotient 1 8
printQuotient 1 2
printQuotient 1 5
printQuotient 1 3        





