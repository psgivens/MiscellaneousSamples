module ForonoiSweep
open ForonoiMath

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
    
let startNode = Node(1, Start, Empty, Empty)
let leaf e = Node(1, e, Empty, Empty)
    
let makeNode e a b = 
    if rank a < rank b then Node(rank a + 1, e, b, a)
    else Node(rank b + 1, e, a, b)
    
let rec merge a b = 
    match a, b with
    | Empty, Empty -> Empty
    | Empty, node -> node
    | node, Empty -> node
    | Node(_, e, l, r), Node(_, e', l', r') -> 
        if e.y < e'.y then makeNode e l (merge r b)
        else makeNode e' l' (merge r' a)
    
let insert e h = merge h <| leaf e
    
let pop = 
    function 
    | Empty -> failwith "Cannot pop from and empty heap"
    | Node(_, e, l, r) -> e, merge l r


