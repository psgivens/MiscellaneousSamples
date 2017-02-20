// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System

let input = 
    [| 
        "..............F....";
        "...A...............";
        "...................";
        "...................";
        "..............C....";
        "......D............";
        "...................";
        ".............B.....";
        ".E..G..............";
        "...H...............";
    |]

let letterLocations =
    input 
    |> Array.mapi (
        fun y row -> 
            row.ToCharArray()
            |> Array.mapi (
                fun x -> 
                function
                | '.' -> None
                | character -> Some(x, y, character)
                )
            |> Array.toList
        )
    |> Array.toList
    |> List.fold (fun acc item -> 
        item 
        |> List.fold (fun acc item -> item::acc) acc
        ) []
    |> List.choose id
    |> List.sortBy (fun (x, y, item) -> item)

letterLocations 
|> List.iter (fun (x, y, item) -> printfn "%c at (%d, %d)" item x y)

type Direction = Left = 1 | Up = 2 | Right = 3 | Down = 0

let otherDirections (direction:Direction)= 
    let intValue = int direction
    [1..3] |> List.map (fun i -> enum<Direction> ((i+intValue) % 4))

type Ownership = Owner of char | Owned of char | Boundary of char | Unknown

let output = 
    Array2D.zeroCreate<Ownership> input.Length input.[0].Length

let directions = 
    [0..3] |> List.map (fun i -> enum<Direction> i)

let moveOne (x,y) direction =
    match direction with
    | Direction.Left ->  x - 1, y
    | Direction.Up ->    x , y - 1
    | Direction.Right -> x + 1, y
    | Direction.Down ->  x , y + 1
    | _ -> failwith "Invalid enum supplied"

let foldBack folder acc list = List.foldBack folder list acc 

let processLetters = 
    foldBack (fun (x, y, item) acc-> 
        output.[y,x] <- Owner(item)
        directions 
        |> foldBack (fun direction acc -> 
            (item, (moveOne (x,y) direction), direction)::acc
            ) acc
        ) []

    >> foldBack (fun (item, (x,y), direction) acc ->
        otherDirections direction
        |> List.fold (fun acc d -> (item, (x,y), d)::acc) acc
        )[]

processLetters letterLocations 
|> ignore

//|> foldBack (fun (item, (x,y), direction acc ->
//    let value = output.[y,x]
//    match value with
//    | Owner(c) -> ()
//    | Owned(c) -> ()
//    | Boundary(c) -> () 
//    | Unknown -> 
//        output.[y,x] <- Owner(item)    
//    acc    
//    ) []
//    // TODO: Process the first block outside of the Owner blocks. 
//|> ignore
    

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    printfn "Press any key to exit"
    Console.ReadKey() |> ignore
    0 // return an integer exit code

