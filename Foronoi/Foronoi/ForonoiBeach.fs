module ForonoiBeach

open ForonoiMath
    
type BeachChanged = 
    { removed : CircleCoord option
      inserted : CircleCoord list }
    
type BeachNode = int * int * CircleCoord 
    
type BeachLine(width : int, height : int) = 

    let mutable left = (0, 0), height
    let mutable vertices = List.empty<BeachNode>

    let coords (x, y, _) = x, y
    
    let (|LessThan|_|) x' = 
        function 
        | (x, _, _) as node when x < x' -> Some(node)
        | _ -> None
    
    let (|GreaterThan|_|) x' = 
        function 
        | (x, _, _) as node when x > x' -> Some(node)
        | _ -> None

    // Returns linked list of vertices
    // Returns removed CircleCoord
    member beach.insert (x, y) = 

        let interceptEdge edge (x'',y'') = 
            let m,b = bisector (x,y) (x'',y'')
            let x''',y''' = intercept (m,b) edge
            let d = distance (x,y) (float x''',float y''')
            (int x''',int y'''), int d + y'''

        let interceptLeft = interceptEdge 0
        let interceptRight = interceptEdge width 

        let rec insert' = function 
                        
            | [] -> 
                // Add first element
                [(x,y,((width,0),height))], 
                { removed = None; inserted = []}

            | LessThan x (x',y',toBeRemoved) :: next :: tail -> 
                // Add before first element when next element exists
                left <- interceptLeft (x',y')
                let rightCirc = calculateCenter (x,y) (x',y') (coords next)
                (x,y,left) :: (x',y',rightCirc) :: [],
                { removed = Some(toBeRemoved); inserted = [left;rightCirc]}

            | LessThan x (x',y',toBeRemoved) :: [] ->            
                // add before first element when next element doesn't exist
                left <- interceptLeft (x',y')
                let rightCirc = interceptRight (x',y')
                (x,y,left) :: (x',y',rightCirc) :: [],
                { removed = Some(toBeRemoved); inserted = [left;rightCirc]}
                
            | GreaterThan x (x', y',toBeRemoved) :: [] ->
                // Add after first element when next element doesn't exist
                let x'' = (x + x')/2
                let radius = distance (x,y) (float x'',0.0)
                let leftCirc = ((x'',0),int radius)
                let rightCirc = interceptRight (x',y')
                (x',y',leftCirc) :: (x,y,rightCirc) :: [],
                { removed = Some(toBeRemoved); inserted = [leftCirc;rightCirc]}

            | pre :: GreaterThan x (x',y',toBeRemoved) :: LessThan x next :: tail -> 
                // Happy path
                // Add in the middle of the list

                // 1. create circle with pre, last and new
                let leftCirc = calculateCenter (coords pre) (x', y') (x, y)
                    
                // 2. create circle with new, next and post
                let rightCirc = 
                    let x'',y'' = coords next
                    match tail with
                    | [] -> interceptRight (x'',y'')
                    | post :: _ -> calculateCenter (x, y) (x'',y'') (coords post)
                pre :: (x', y', leftCirc) :: (x, y, rightCirc) :: next :: tail,
                { removed = Some(toBeRemoved); inserted = [leftCirc;rightCirc]}
                
            | pre :: GreaterThan x (x',y',toBeRemoved) :: [] -> 
                // Inserting after the last element
                let leftCirc = calculateCenter (coords pre) (x', y') (x, y)
                let rightCirc = interceptRight (x',y')
                pre :: (x', y', leftCirc) :: (x, y, rightCirc) :: [],
                { removed = Some(toBeRemoved); inserted = [leftCirc;rightCirc]}

            | list -> insert' list
        
        let vertices', delta = insert' vertices
        vertices <- vertices'
        delta

