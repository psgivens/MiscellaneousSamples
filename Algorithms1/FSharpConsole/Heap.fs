namespace Heaps

type HeapNode = 
    | Tree of (int * int * HeapNode * HeapNode)
    | Empty

module Heap = 
    let leaf value = 
        Tree(value, 0, Empty, Empty)

    let makeNode element nodeA nodeB = 
        match nodeA, nodeB with
        | Tree(_, r, _, _), Empty -> Tree(element, r, nodeA, Empty)
        | Empty, Tree(_, r, _, _) -> Tree(element, r, nodeB, Empty)
        | Empty, Empty ->            leaf element
        | Tree(_, ra, _, _), Tree(_, rb, _, _) -> 
            if (ra > rb) then Tree(element, ra + 1, nodeA, nodeB)
            else Tree(element, rb + 1, nodeB, nodeA)
    
    let rec merge nodeA nodeB = 
        match nodeA, nodeB with
        | Tree(_, _, _, _), Empty -> nodeA
        | Empty, Tree(_, _, _, _) -> nodeB
        | Empty, Empty ->            Empty
        | Tree(e1, _, a1, b1), Tree(e2, _, a2, b2) -> 
            if (e1 < e2) then        makeNode e1 a1 (merge b1 nodeB)
            else                     makeNode e2 a2 (merge b2 nodeA)
    
    let rec insert heap value = 
        merge heap (leaf value)

    let pop heap = 
        match heap with 
        | Empty ->          None
        | Tree(e,r,a,b) ->  Some(e, merge a b)



//let doit x =
//    match x with
//    | 3 -> None
//    | _ -> Some(x)


