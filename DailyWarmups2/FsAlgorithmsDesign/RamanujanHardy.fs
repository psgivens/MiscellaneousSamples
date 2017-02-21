module RamanujanHardy

let length = Seq.length >> printfn "Length %i"

let total = 10000
let numbers = [|1..total|]
let cubes = numbers |> Array.map (fun x -> x*x*x)
numbers |> length

       
let rec checkCase lower upper offset offset':(Option<int>*bool)= 
    let left = cubes.[lower] + cubes.[upper]
    let right = cubes.[offset] + cubes.[offset']
    if (left = right) then (Some(upper),true)
    elif (left > right) then (None,true)
    else (None,false)
    
let values =
    [3..(total-1)]
    |> Seq.map (fun i -> 
        let checkCase' = checkCase 0 i
        [(i-1).. -1 .. 2] 
        |> Seq.fold (fun acc'' o ->
            match acc'' with 
            | Some(value), true -> acc''
            | _,_ -> 
                let checkCase'' = checkCase' o
                [(o-1) .. -1 .. 1] 
                |> Seq.fold (fun acc item ->
                    match acc with
                    | _, false -> checkCase'' item
                    | _ -> acc
                ) acc''
            ) (None, false)
        |> fst
        ) 
    |> Seq.filter Option.isSome
    |> Seq.map Option.get
    
values |> Seq.iter (printfn "%i ")    
//    iter first 3 []


//let candidates = 
//    cubes 
//    |> List.windowed 3
//    |> List.filter (function 
//        | [first;second;third] when second+third >= first + 1 -> true
//        | _ -> false)


//candidates |> length

