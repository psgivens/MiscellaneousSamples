

type Rank = int
type Element = int
type Heap = 
    | Empty
    | Node of Rank * Element * Heap * Heap

let newNode e =
    Node (1,e,Empty, Empty)

let rank = function
    | Empty -> 0
    | Node(r,_,_,_) -> r

let makeNode e a b =
    if rank a < rank b then Node (rank a + 1, e, b, a)
    else                    Node (rank b + 1, e, a, b)

let rec merge a b =
    match a,b with
    | Empty, Empty -> Empty
    | node, Empty -> node
    | Empty, node -> node
    | Node(_,e',a',b'), Node(_,e'',a'',b'') ->
        if e' < e'' then makeNode e' a' <| merge b b'
        else             makeNode e'' b'' <| merge a'' a

let insert e heap =
    merge heap <| newNode e
    
let pop = function
    | Empty         -> -1,Empty
    | Node(_,e,a,b) -> e, merge a b

let popOption = function
    | Empty         -> None
    | Node(_,e,a,b) -> Some(e, merge a b)

let insertFold heap e = 
    insert e heap
    

[1..4..100]@[99..-3..1]
|> List.fold insertFold Empty 
|> Seq.unfold popOption
|> Seq.iter (printf "%d, ")

    
        