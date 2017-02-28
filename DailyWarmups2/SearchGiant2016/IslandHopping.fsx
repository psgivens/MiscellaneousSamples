//module IslandHopping

type Coords = int * int
type Direction = | Left | Up | Right | Down 
let allDirections = [Left;Up;Right;Down]

let calculateOdds rows columns stormCount = 
    let increment count countMap =
        match countMap |> Map.tryFind count with
        | None -> countMap |> Map.add count 1
        | Some(n) -> countMap |> Map.add count (n + 1)

    let oddsOfBridgeSurvival = pown 0.5 stormCount

    let rec countPaths' countMap (traversed:Coords list) count (x,y) = 
        // Success, we hit the other side.
        if y > rows then countMap |> increment count

        // Out of bounds, we fell into the ocean
        elif x <= 0 || x > columns || y < 1 then countMap

        // We've already been here, don't count it. 
        elif traversed |> List.contains (x,y) then countMap
        
        // Check the next direction
        else 
            allDirections 
            |> List.map (
                function
                | Direction.Left  -> x-1, y
                | Direction.Up    -> x, y-1
                | Direction.Right -> x+1, y
                | Direction.Down  -> x, y+1)
            |> List.fold (fun countMap' nextPoint ->            
                countPaths' 
                    countMap'
                    ((x,y)::traversed)
                    (count + 1)
                    nextPoint) countMap

    // For each island adjacent to the shore
    [1..columns]
    |> List.fold (fun acc x -> countPaths' acc [] 1 (x,1)) Map.empty<int,int>
    |> Map.toList

    // Calculate odds based on number of paths at different lengths
    |> List.fold (fun acc (length,count) ->
        let oddsOfPathDestroyed = 1.0 - (pown oddsOfBridgeSurvival length)
        acc * (pown oddsOfPathDestroyed count)) 1.0
    |> (-) 1.0

let print rows columns stormCount =    
    printfn "%d %d %d %f" rows columns stormCount 
        <| calculateOdds rows columns stormCount 
print 3 1 1
print 3 2 1
print 3 2 2


    


