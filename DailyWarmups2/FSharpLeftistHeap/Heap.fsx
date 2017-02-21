

type HeapNode  =
    | Empty
    | Node of int * int * HeapNode * HeapNode

let rank = function 
    | Empty -> 0
    | Node(r,_,_,_) -> r
    
let makeNode (e, a, b) =
    if rank a < rank b then Node(rank a + 1, e, b, a)
    else                    Node(rank b + 1, e, a, b)

let rec merge a b =
    match a,b with
    | Empty, Empty -> Empty
    | Empty, node 
    | node, Empty -> node
    | Node(_,ae,al,ar), Node(_,be,bl,br) ->
        if (ae < be) then makeNode (ae, al, merge ar b) 
        else makeNode (be, bl, merge br a)

let insert e h =
    (e, Empty, Empty)
    |> makeNode
    |> merge h 

let pop = function
    | Empty -> failwith "Cannot pop and empty heap"
    | Node(_,e,l,r) -> (e, merge l r)

let rec unpack = function 
    | Empty -> printfn "Empty"
    | node -> 
        let e,n = pop node
        printfn "%d" e
        unpack n

//makeNode (13, Empty, Empty)
let a = Empty |> insert 13
let b = a |> insert 14
let c = b |> insert 21
let d = c |> insert 12

let e = [11..-1..1] |> List.fold (fun acc item -> insert item acc) d
let g = [15..20] |> List.fold (fun acc item -> insert item acc) e


unpack g




