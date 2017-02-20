module Example 

type HeapNode = | Empty | Node of (int * int * HeapNode * HeapNode) 
    
module Heap = 
    
    let add x y = x + y
    
    let makeNode element nodeA nodeB =
        match nodeA, nodeB with
        | Node(e, r, a, b), Empty ->    Node(element, r, nodeA, Empty)
        | Empty, Node(e, r, a, b) ->    Node(element, r, nodeB, Empty)
        | Empty, Empty ->               Node(element, 0, nodeA, nodeB)
        | Node(e1, r1, a1, b1), Node(e2, r2, a2, b2) -> 
            if (r1 < r2) then           Node (element, r2 + 1, nodeB, nodeA)
            else                        Node(element, r1 + 1, nodeA, nodeB)

    let rec merge nodeA nodeB =
        match nodeA, nodeB with
        | Node(_,_,_,_), Empty -> nodeA
        | Empty, Node(_,_,_,_) -> nodeB
        | Empty, Empty -> Empty
        | Node(e1, r1, a1, b1), Node(e2, r2, a2, b2) -> 
            if (e1 < e2) then   makeNode e1 a1 (merge b1 nodeB)
            else                makeNode e2 a2 (merge b2 nodeA)  

    let insert node element =
        merge node (makeNode element Empty Empty)

    let removeHead node =
        match node with 
        | Empty -> Empty
        | Node(e, r, a1, b1) -> (merge a1 b1)

    let pop node =
        match node with
        | Empty -> Empty, 0
        | Node(e, r, a1, b1) -> (merge a1 b1), e

    let rec recFoldBack node cont =
        match node with
        | Empty -> cont []
        | Node(e,r,a,b) -> recFoldBack (merge a b) (fun list -> cont(e::list))

    let toList node =
        recFoldBack node (fun list -> list)
       
module List =
    let toHeap list =
        list |>
        List.fold (fun acc item -> Heap.insert acc item) HeapNode.Empty
            
