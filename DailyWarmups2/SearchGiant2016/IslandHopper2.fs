module IslandHopper2

open NUnit.Framework
open FsUnit

(*
    This is a permutatin problem. 
    Traverse every path, count the nodes.
    Do the math. 

    to traverse every path, we must consider left, right and straight. 
    we must exclude visited nodes. 

    r c s answer
    1 1 1 0.250000
    2 1 1 0.125000
    1 2 1 0.569336
    1 1 2 0.062500
    3 1 1 0.062500
    3 2 1 0.349325
    3 2 2 0.015161
*)

type Direction = | Left | Up | Right | Down

let calculateOdds rows columns stormCount = 
    let countMap = Map.empty<int,int>
    let increment length map =
        match map |> Map.tryFind length with
        | None -> map |> Map.add length 1
        | Some(value) -> map |> Map.add length (value + 1)

    let rec countPaths countMap path length (x,y) =
        if x < 0 || x >= columns || y < 0 then countMap
        elif y >= rows then countMap |> increment length
        else             
            [Left;Up;Right;Down]
            |> List.fold (fun map dir ->
                let pos = 
                    match dir with
                    | Left  -> x-1,y
                    | Up    -> x,y+1
                    | Right -> x+1,y
                    | Down  -> x,y-1
                if path |> List.exists(fun item -> item = pos) then map
                else countPaths map ((x,y)::path) (length+1) pos
                ) countMap

    let oddsOfBridgeOut = 1.0 - (pown 0.5 stormCount)
    let oddsMap = 
        [1..10]
        |> List.fold (fun acc i -> 
            let previousLength,oddsOfBridgeDestroyed = acc |> List.head
            (i,(float oddsOfBridgeDestroyed) + oddsOfBridgeOut * (1.0-oddsOfBridgeDestroyed))::acc
            ) [(0,0.0)]
        |> Map.ofList

    let calculateAny oddsOfOne possibilityCount = 
        [1..possibilityCount]
        |> List.fold (fun acc i -> 
            let previousLength,oddsOfBridgeDestroyed = acc 
            (i,(float oddsOfBridgeDestroyed) + oddsOfBridgeOut * (1.0-oddsOfBridgeDestroyed))
            ) (0,0.0)
        |> snd

    [0..columns-1]
    |> List.fold (fun map c -> countPaths map [] 1 (c,0)) countMap
    |> Map.toList
    |> List.fold (fun oddsOfCrossingAnotherWay (bridgesInSequence, count) ->
        let oddsOfCrossing1Path = 1.0 - oddsMap.[bridgesInSequence]
        let oddsOfCrossingAny = oddsOfCrossing1Path * float count
        oddsOfCrossingAnotherWay + oddsOfCrossingAny - oddsOfCrossingAnotherWay * oddsOfCrossingAny
        ) 0.0


[<Test>]
let ``test 1 row, 1 column, and 1 storm`` () =
    calculateOdds 1 1 1 
    |> should equal 0.25

[<Test>]
let ``test 2 row, 1 column, and 1 storm`` () =
    calculateOdds 2 1 1
    |> should equal 0.125

[<Test>]
let ``test 1 row, 1 column, and 2 storms`` () =
    calculateOdds 1 1 2
    |> should equal 0.06250

[<Test>]
let ``test 1 row, 2 columns, and 1 storm`` () =
    calculateOdds 1 2 1 
    |> should equal 0.569336


//[<Test>]
//let ``test 1 storm, 1 row, and 2 column`` () =
//    calculateOdds 1 2 1 
//    |> should equal 0.25
//
//[<Test>]
//let ``test 1 storm, 1 row, and 2 column`` () =
//    calculateOdds 1 2 1 
//    |> should equal 0.25
//
//[<Test>]
//let ``test 1 storm, 1 row, and 2 column`` () =
//    calculateOdds 1 2 1 
//    |> should equal 0.25
//
    //
    //
    //1 1 1 0.250000
    //2 1 1 0.125000
    //1 2 1 0.569336
    //1 1 2 0.062500
    //3 1 1 0.062500
    //3 2 1 0.349325
    //3 2 2 0.015161
    //
