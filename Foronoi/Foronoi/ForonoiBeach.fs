module ForonoiBeach

open ForonoiMath
    
type BeachChanged = 
    { removed : CircleCoord list
      inserted : CircleCoord list }
    
type BeachNode = int * int * CircleCoord 
    
type BeachLine(width : int, height : int) = 

    let mutable left = (0, 0), height
    let mutable vertices = List.empty<BeachNode>

    let coords (x, y, _) = x, y
    
    let (|LessThan|_|) x = 
        function 
        | (x', _, _) as node when x < x' -> Some(node)
        | _ -> None
    
    let (|GreaterThan|_|) x = 
        function 
        | (x', _, _) as node when x > x' -> Some(node)
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
                { removed = []; inserted = []}

            | LessThan x (x',y',toBeRemoved') :: (x'',y'',toBeRemoved'') :: tail -> 
                // Add before first element when next element exists
                left <- interceptLeft (x',y')

                let centerCirc = calculateCenter (x,y) (x',y') (x'',y'')
                let rightCirc = match tail with
                                | [] -> interceptRight (x'',y'')
                                | post :: _ -> calculateCenter (x, y) (x'',y'') (coords post)

                (x,y,left) :: (x',y',centerCirc):: (x'',y'',rightCirc) :: [],
                { removed = [toBeRemoved';toBeRemoved'']; inserted = [left;centerCirc;rightCirc]}

            | LessThan x (x',y',toBeRemoved) :: [] ->            
                // Add before first element when next element doesn't exist

                let left' = left
                left <- interceptLeft (x',y')

                // Between two points, we run the chance of hitting the boundaries.
                // This should be calculated into all 
                let x'' = (x + x')/2
                let centerCirc = ((x'',0),height)
                
                let rightCirc = interceptRight (x',y')
                (x,y,centerCirc) :: (x',y',rightCirc) :: [],
                { removed = [toBeRemoved;left']; inserted = [left;centerCirc;rightCirc]}
                
            | GreaterThan x (x', y',toBeRemoved') :: [] ->
                // Add after first element when next element doesn't exist
                let left' = left
                left <- interceptLeft (x',y')

                // Between two points, we run the chance of hitting the boundaries.
                // This should be calculated into all 
                let x'' = (x + x')/2
                let centerCirc = ((x'',0),height)
                
                let rightCirc = interceptRight (x',y')
                (x',y',centerCirc) :: (x,y,rightCirc) :: [],
                { removed = [toBeRemoved';left']; inserted = [left;centerCirc;rightCirc]}

            | pre :: GreaterThan x (x',y',toBeRemoved') :: LessThan x (x'',y'',toBeRemoved'') :: tail -> 
                // Happy path
                // Add in the middle of the list

                let leftCirc = calculateCenter (coords pre) (x', y') (x, y)
                let centerCirc = calculateCenter (x',y') (x,y) (x'',y'')                    
                let rightCirc = match tail with
                                | [] -> interceptRight (x'',y'')
                                | post :: _ -> calculateCenter (x, y) (x'',y'') (coords post)

                pre :: (x', y', leftCirc) :: (x, y, centerCirc) :: (x'',y'',rightCirc) :: tail,
                { removed = [toBeRemoved';toBeRemoved'']; inserted = [leftCirc;rightCirc]}
                
            | pre :: GreaterThan x (x',y',toBeRemoved) :: [] -> 
                // Inserting after the last element
                let leftCirc = calculateCenter (coords pre) (x', y') (x, y)
                
                let rightCirc = interceptRight (x',y')
                pre :: (x', y', leftCirc) :: (x, y, rightCirc) :: [],
                { removed = [toBeRemoved]; inserted = [leftCirc;rightCirc]}

            | list -> insert' list
        
        let vertices', delta = insert' vertices
        vertices <- vertices'
        delta

