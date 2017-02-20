module CoordFornoi

let coords = 
    [ (160, 317)
      (59, 299)
      (241, 251)
      (87, 185)
      (175, 83)
      (275, 277)
      (386, 325)
      (381, 390)
      (111, 76)
      (75, 201)
      (342, 198)
      (33, 52)
      (190, 342)
      (154, 256)
      (14, 138)
      (152, 51)
      (234, 111)
      (253, 78)
      (127, 37)
      (318, 125) ]

let sorted = coords |> List.sortBy snd

type Coord = int * int

type CircleCoord = Coord * int

module ForonoiSweep = 
    type ForonoiEvent = 
        | Start
        | Vertex of Coord
        | Circle of Coord * int
        | Stop of int
        member this.y = 
            match this with
            | Start -> 0
            | Vertex(_, y) -> y
            | Circle((_, _), topY) -> topY
            | Stop(top) -> top
    
    type Rank = int
    
    type Element = ForonoiEvent
    
    type HeapNode = 
        | Empty
        | Node of Rank * Element * HeapNode * HeapNode
    
    let rank = 
        function 
        | Empty -> 0
        | Node(r, _, _, _) -> r
    
    let makeNode e = Node(1, e, Empty, Empty)
    
    let makeT e a b = 
        if rank a < rank b then Node(rank a + 1, e, b, a)
        else Node(rank b + 1, e, a, b)
    
    let rec merge a b = 
        match a, b with
        | Empty, Empty -> Empty
        | Empty, node -> node
        | node, Empty -> node
        | Node(_, e, l, r), Node(_, e', l', r') -> 
            if e.y < e'.y then makeT e l (merge r b)
            else makeT e' l' (merge r' a)
    
    let insert e h = merge h <| makeNode e
    
    let pop = 
        function 
        | Empty -> failwith "Cannot pop from and empty heap"
        | Node(_, e, l, r) -> e, merge l r

module ForornoiBeach = 
    type BeachVertex = 
        { x : int
          Next : BeachFocus }
    
    and BeachFocus = 
        { x : int
          Next : BeachVertex option }
    
    type NodeInserted = 
        { removed : CircleCoord
          inserted : CircleCoord * CircleCoord }
    
    type BeachNode = int * int * CircleCoord
    
    let coords (x, y, _) = x, y
    
    let (|LessThan|_|) x' = 
        function 
        | (x, _, _) as node when x < x' -> Some(node)
        | _ -> None
    
    let (|GreaterThan|_|) x' = 
        function 
        | (x, _, _) as node when x > x' -> Some(node)
        | _ -> None
    
    type BeachLine(width : int, height : int) = 
        let left = (0, 0), height
        let right = (width, 0), height
        let vertices = List.empty<BeachNode>
        
        (*
                |(x'^2   + y'^2  )   y'    1|
                |(x''^2  + y''^2 )   y''   1|
                |(x'''^2 + y'''^2)   y'''  1|
            h = -----------------------------
                        |x'    y'    1|
                    2 * |x''   y''   1|
                        |x'''  y'''  1|

                |x'    (x'^2   + y'^2  )  1|
                |x''   (x''^2  + y''^2 )  1|
                |x'''  (x'''^2 + y'''^2)  1|
            k = -----------------------------
                        |x'    y'    1|
                    2 * |x''   y''   1|
                        |x'''  y'''  1|

                           | a b c |
            determinant =  | d e f |
                           | g h i |
        *)
        let calculateCenter (x', y') (x'', y'') (x''', y''') = 
            let determinant a b c d e f g h i = a * e * i + b * f * g + c * d * h - g * e * c - h * f * a - i * d * b
            let sumSqr x y = x * x + y * y
            let ss' = sumSqr x' y'
            let ss'' = sumSqr x'' y''
            let ss''' = sumSqr x''' y'''
            let divisor = 2 * determinant x' y' 1 x'' y'' 1 x''' y''' 1
            
            let h = 
                let dividend = determinant ss' y' 1 ss'' y'' 1 ss''' y''' 1
                (float dividend) / (float divisor)
            
            let k = 
                let dividend = determinant x' ss' 1 x'' ss'' 1 x''' ss''' 1
                let divisor = 2 * determinant x' y' 1 x'' y'' 1 x''' y''' 1
                (float dividend) / (float divisor)
            
            let square x = x * x
            let radius = sqrt <| square (float x' - h) + square (float y' - k)
            let topY = radius + k
            (int h, int k), int topY

        let bisector (x',y') (x'',y'') =
            let midx, midy = (x'+x'')/2, (y'+y'')/2
            let s = (y''-y') / (x''-x')
            let m = s/1
            let b = midy - m * midx
            (m,b)
        
        // Returns linked list of vertices
        // Returns removed CircleCoord
        member beach.insert (x, y) = 
            let rec insert' = 
                function 
                | pre :: GreaterThan x last :: LessThan x next :: tail -> 
                    // normal case
                    // 1. create circle with pre, last and new
                    let (x', y', _) = last
                    let leftCirc = calculateCenter (coords pre) (x', y') (x, y)
                    
                    // 2. create circle with new, next and beyond
                    let rightCirc = 
                        match tail with
                        | [] -> 
                            // TODO: Calculate based on right boundary
                            // TODO: Calculate perpindicular bisector of two points
                            (x, y), 3
                        | post :: _ -> calculateCenter (x, y) (coords next) (coords post)
                    pre :: (x', y', leftCirc) :: (x, y, rightCirc) :: next :: tail
                // inserting after the last element
                // Calculate based on right boundary
                | pre :: GreaterThan x last :: [] -> []
                | [] -> []
                // insert the second item
                | head :: [] -> [] // insert backwoards from end
                | head :: tail -> 
                    let (x', y', _) = head
                    if x' > x then head :: insert' tail
                    else 
                        // calculate new circle events
                        // prepend new virtex and circles
                        let (x'', y'', circleToRemove) = head
                        // 1. Find the center of three points
                        // http://mathforum.org/library/drmath/view/55239.html
                        tail
            match vertices with
            | LessThan x first :: tail -> []
            // add before first element
            // Calculate based on left boundary
            | [] -> []
            // add first element
            | _ -> insert' vertices
