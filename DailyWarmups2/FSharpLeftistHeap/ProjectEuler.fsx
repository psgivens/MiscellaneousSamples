

Seq.unfold (function
    | 0, 0 ->   Some(1, (0,1))
    | x, y  -> 
        match x+y with
        | next when x > 4000000 -> None
        | next -> Some(next,(y,next))
    ) (0,0)
|> Seq.filter (fun x -> x % 2 = 0)
|> Seq.takeWhile (fun x -> x < 4000000)
|> Seq.sum

seq { 999 .. -1 .. 100 }
|> Seq.map (fun x ->
    let str = x.ToString().ToCharArray() |> Array.toList 
    let revstr = new string (str |> List.rev |> (@) str |> List.toArray)
    System.Int32.Parse(revstr)
    )
|> Seq.find (fun x ->
    match [999 .. -1 .. (x/999)]
        |> List.tryFind (fun y -> 
            x % y = 0 && x / y < 1000)
        with
        | None -> false 
        | Some(value) -> true    
    )


