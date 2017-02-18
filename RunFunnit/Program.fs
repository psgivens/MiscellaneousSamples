
// NOTE: If warnings appear, you may need to retarget this project to .NET 4.0. Show the Solution
// Pad, right-click on the project node, choose 'Options --> Build --> General' and change the target
// framework to .NET 4.0 or .NET 4.5.

module Funnit.Library

    // The first thing that we need is some data. 
    let originalData = [
         "........................."
         "............B............" 
         "........................." 
         "........A................" 
         "........................." 
         "................C........"          
         "........................." 
         "...................G....." 
         ".......D................." 
         "........F................"           
         ".......E................."          
         "........................."
         "........................."
         "........................."
         ]

    // Now we need to convert that data to a double dimension 
    // array so that we can access it through indexers.          
    let dataMatrix = 
        originalData
        |> List.map (fun st -> st.ToCharArray())
        |> List.toArray

    // We are going to need a concept of ownership for each
    // cell. 
    type Owned = Unclaimed | Owner of char | Claimed of char | Boundary of char

    // Let us create a matrix representing the ownership of each
    // cell
    let claims =
        dataMatrix
        |> Array.map (fun row ->
            row
            |> Array.map (function
                | '.' -> Owned.Unclaimed
                | ch -> Owned.Owner(ch)))
    
    // Let us have a utility method to see what has happened.         
    let printIt () =
        printfn "" // prints a new line
        claims
        |> Array.iter (fun row ->
            row |> Array.iter (function
                | Owned.Claimed(ch) -> printf " ."   
                | Owned.Owner(ch) -> printf " %c" ch
                | Owned.Boundary(ch) -> printf " %c" ch 
                | _ -> printf " ." )
            printfn "")            

    // Let us create a record to represent where a partuclar
    // capital letter resides. 
    type CapitalLocation = { X:int; Y:int; Letter:char }

    // Now we want to find all of the capital letters. 
    let capitals = 
        dataMatrix
        |> Array.mapi (fun y row -> 
            row 
            |> Array.mapi (fun x item -> 
                match item with
                | '.' -> None
                | _ -> Some({ X=x; Y=y; Letter=item }))
            |> Array.choose id
            |> Array.toList)
        |> Array.fold (fun acc item -> item @ acc) List.empty<CapitalLocation>
        |> List.sortBy (fun item -> item.Letter)

    // As we move about, we need a concept of direction. 
    type Direction = Left = 0 | Up = 1 | Right = 2 | Down = 3   
         
    // Function gets the coordinates of the adjacent cell. 
    let getCoordinates (x, y) direction =
        match direction with
        | Direction.Left -> x-1, y
        | Direction.Up -> x, y-1
        | Direction.Right -> x+1, y
        | Direction.Down -> x, y+1
        | _ -> (-1,-1) // TODO: Figure out how to best throw an error here. 
             
    // As we move about, we are going to need to know about size 
    // This will help us monitor whether we are moving out of bounds. 
    type Size = { Width:int; Height: int }    
    
    // Get the size of the matrix. 
    let size = {Width=originalData.Head.Length; Height=originalData.Length}
     
    // Active Pattern: matches criteria of a given cell.
    let (|OutOfBounds|UnclaimedCell|Claimed|Boundary|Owned|) (x,y) =
        match (x,y) with 
        | _,_ when x < 0 || y < 0 -> OutOfBounds
        | _,_ when x >= size.Width || y >= size.Height -> OutOfBounds
        | _ ->                     
            match claims.[y].[x] with
            | Owned.Unclaimed -> UnclaimedCell(x,y)
            | Owned.Claimed(ch) -> Claimed(x,y,ch)
            | Owned.Boundary(ch) -> Boundary(x,y,ch)
            | Owned.Owner(ch) -> Owned(x,y,ch)
        
    // Now we are getting down to brass tax. This claims the cell!
    let claimCell letter (x, y) =         
        // Side effect: Change the value of the cell
        (claims.[y].[x] <- Owned.Claimed (System.Char.ToLower letter)) |> ignore
         
    // Using the active pattern, claim this cell if unclaimed,
    // and return the coordinates of the adjacent cells.           
    let claimAndReturnAdjacentCells (letter, coordinates, direction) =
        match coordinates with 
        | UnclaimedCell (x,y) ->
            // Claim it and return the Owned object.
            claimCell letter coordinates // meaningful side effect
            // use Direction as int to allow math to be performed. 
            let directionInt = int direction;            
            Some(
                // [counter-clockwise; forward; clockwise]
                [(directionInt+3)%4; directionInt; (directionInt+1)%4] 
                |> List.map enum<Direction>                 
                |> List.map (fun newDirection -> 
                    (
                        letter, 
                        getCoordinates coordinates newDirection, 
                        newDirection
                    )))
        | Claimed(cx,cy,cch) when cch <> System.Char.ToLower letter-> 
            // If it is claimed and we are trying to convert it, it must be
            // a boundary cell. 
            (claims.[cy].[cx] <- Owned.Boundary (System.Char.ToLower cch)) |> ignore
            None                                      
        | _ -> None

    // We are starting to create lists of this data bag, let
    // us create a type to make things clearer. 
    type CellClaimCriteria = (char * (int * int) * Direction)
     
    // Given a list of criterial for claiming cells, we will
    // iterate over the list returning the next cells to claim
    // and recurse into that list. 
    let rec claimCells (items:CellClaimCriteria list) =
        items
        |> List.fold (fun acc item ->
            let results = claimAndReturnAdjacentCells item 
            if Option.isSome(results) 
            then (acc @ Option.get results) 
            else acc
            ) List.empty<CellClaimCriteria> 
        |> (fun l ->            
            match l with
            | [] -> []
            | _ -> claimCells l)
        
    // For each capital, create a claim criteria in each direction 
    // and then recursively claim those cells. 
    let claimCellsFromCapitalsOut ()=
        capitals
        |> List.fold (fun acc capital ->
            let getCoordinates = getCoordinates (capital.X, capital.Y)
            [Direction.Left; Direction.Up; Direction.Right; Direction.Down]
            |> List.map (fun direction ->                
                (
                    capital.Letter, 
                    getCoordinates direction, 
                    direction
                ))
            |> (fun items -> acc @ items)
            ) List.empty<CellClaimCriteria>
        |> claimCells

    [<EntryPoint>]
    let main args = 
        printIt()
        claimCellsFromCapitalsOut()
        printIt()
        0

