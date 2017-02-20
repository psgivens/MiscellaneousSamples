

module Heap1

type Element = int
type Rank = int
type Heap =
    | Empty
    | Node of Element * Rank * Heap * Heap

let rank = function
    | Empty -> 0
    | Node(_,r,_,_) -> r

let leaf (e:int) =
    Node(e, 1, Empty, Empty)

let makeT (e, a, b) = 
    if rank a < rank b then Node(e, rank a + 1, b, a)
    else                    Node(e, rank b + 1, a, b)

let rec merge a b =
    match a,b with
    | Empty, Empty -> Empty
    | Empty, node -> node
    | node, Empty -> node
    | Node(e1,_,l1,r1), Node(e2,_,l2,r2) ->
        if e1 < e2 then makeT (e1,l1, merge r1 b)
        else makeT (e2, l2, merge r2 a)
        
let insert n h =
    merge h <| leaf n

let pop = function
    | Empty -> (-1, Empty)
    | Node(e,_,l,r) -> (e, merge l r)

let printH = function
    | Empty -> printfn "  Empty"
    | Node(_,_,l,r) -> printfn "  %i %i" (rank l) (rank r)

[1..10..101]@[102..-10..2]@[53..60]
|> List.fold (fun acc i ->
    printfn "%d %d" (rank acc) i
    printH acc
    insert i acc) Empty
|> Seq.unfold (fun state -> 
    match pop state with 
    | (-1,_) -> None
    | more -> Some(more))
|> Seq.toList

|> Seq.take 200



