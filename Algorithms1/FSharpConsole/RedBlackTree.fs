namespace rbtree

type Color = 
    | R 
    | B 
    // Double black is a transitionsl state. It represents
    | DB 
type Tree = 
    | E 
    | T of Color * int * Tree * Tree 
    // Sentinel is a transitional state. . It represents an 
    // empty node which counts as two black. Rebalancing is 
    // in order to move one black to another location. 
    | S  


module RedBlackTree =
    let color = function 
        | E -> B
        | T(c,v,l,r) -> c
        | S -> DB

    let rec isMember value tree = 
        match value, tree with
        | x, E -> false
        | x, T(_,y,a,b) ->
            if x < y then isMember x a
            else if x > x then isMember x b
            else true
        | _ -> false

    let rec validate tree = 
        let rec count i =             
            function
            | E -> i
            | S -> failwith "Should not be counting with sentinels"
            | T(_,_,T(B,_,_,_),E) 
            | T(_,_,E,T(B,_,_,_)) -> failwith "Black cannot have an Empty sibbling"
            | T(B,_,E,E) -> i + 1
            | T(R,_,E,E) -> i            
            | T(_,_,T(R,_,T(_),c),E) -> failwith "Red sibling of Empty cannot have children"
            | T(_,_,T(R,_,_,T(_)),E) -> failwith "Red sibling of Empty cannot have children"

            | T(B,_,T(R,_,_,_),E)  
            | T(B,_,E,T(R,_,_,_)) -> i + 1
            | T(R,_,E,T(R,_,_,_)) 
            | T(R,_,T(R,_,_,_),E) -> i
            
            | T(c',x,a,b) ->
                let lcount = count i a 
                if lcount <> count i b then failwith "children must match"
                else lcount
        count 0 tree |> ignore
        
    let (|DoubleBlack|_|) rb =
        match rb with
        | T(DB,x,a,b) -> Some(T(B,x,a,b))
        | S -> Some(E)
        | _ -> None

    let rec private balance f tree =
        let balance' = balance f
        match tree with
        (* Double Red *)
        | T(B,z,T(R,y,T(R,x,a,b),c),d) -> T(R,y,T(B,x,a,b),T(B,z,c,d))
        | T(B,z,T(R,x,a,T(R,y,b,c)),d) -> T(R,y,T(B,x,a,b),T(B,z,c,d))
        | T(B,x,a,T(R,z,T(R,y,b,c),d)) -> T(R,y,T(B,x,a,b),T(B,z,c,d))
        | T(B,x,a,T(R,y,b,T(R,z,c,d))) -> T(R,y,T(B,x,a,b),T(B,z,c,d))
        
        (* Double Black on left side *)
        // Red Sibling
        | T(B,x,DoubleBlack replacement,T(R,y,a,b))         -> T(B,y,T(B,f x,replacement,a),b) 
        // Red distal nephew
        | T(B,x,DoubleBlack replacement,T(B,y,a,T(R,z,b,c)))-> T(B,y,T(B,f x,replacement,a),T(B,z,b,c))
        // Red proximal nephew
        | T(B,x,T(DB,y,a,b),T(B,z,T(R,w,c,d),t'))           -> balance'(T(B,f x,T(DB,y,a,b),T(B,w,c,T(R,z,d,t'))))
        // Without red nephews
        | T(B,x,DoubleBlack replacement,T(B,y,a,b))         -> balance'(T(DB,f x,replacement,T(R,y,a,b)))
        // Red parent
        | T(R,x,DoubleBlack replacement,T(B,y,a,b))         -> balance'(T(B,f x,replacement,T(R,y,a,b)))

        (* Double Black on right side *)
        // Red Sibling
        | T(B,x,T(R,y,a,b),DoubleBlack replacement)         -> T(B,y,a,T(B,f x,b,replacement)) 
        // Red distal nephew
        | T(B,x,T(B,y,T(R,z,a,b),c),DoubleBlack replacement)-> T(B,y,T(B,z,a,b),T(B,f x,c,replacement))
        // Red proximal nephew
        | T(B,x,T(B,y,t',T(R,z,a,b)),T(DB,w,c,d))           -> balance'(T(B,f x,T(B,z,T(R,y,t',a),b),T(DB,w,c,d)))
        // Without red nephews
        | T(B,x,T(B,y,a,b),DoubleBlack replacement)         -> balance'(T(DB,f x,T(R,y,a,b),replacement))
        // Red parent
        | T(R,x,T(B,y,a,b),DoubleBlack replacement)         -> balance'(T(B,f x,T(R,y,a,b),replacement))

        // Otherwise return the same tree. 
        | tree -> tree

    let private trySwap i j k =
        if i = k then j else k
        
    let private root = function
        | T(DB,x,a,b) -> T(B,x,a,b)
        | T(R,x,a,b)  -> T(B,x,a,b)
        | tree -> tree

    let remove element tree =
        let trySwap' = trySwap element

        // The purpose of 'swap and remove' is to swap the element to be removed
        // with a leaf node element. The leaf is then easily removed by not building
        // it into the new tree. 
        
        // Find and remove the right most ancestor.
        let rec swapAndRemoveRightMost = function
            // Look for element where right child is E.

            // Remove Red from Black - No need to rebalance
            | T(B,x,a,T(R,toBeSwappedUp,b,E)) -> T(B,x,a,b),toBeSwappedUp
               
            // Remove Black with no children - Introduce sentinel
            | T(c',x,a,T(B,toBeSwappedUp,E,E)) -> balance (trySwap' toBeSwappedUp) (T(c',x,a,S)),toBeSwappedUp

            // Remove Black with Red child from any parent (*parent not shown) - Promote child and preserve blackness
            | T(B,toBeSwappedUp,T(R,z,E,E),E) -> T(B,z,E,E),toBeSwappedUp

            // Continue to the right most. 
            | T(c',x,a, T(c'',y,b,c)) -> 
                let t,toBeSwappedUp = swapAndRemoveRightMost (T(c'',y,b,c))
                let trySwap'' = trySwap' toBeSwappedUp
                balance trySwap'' (T(c',x,a,balance trySwap'' t)),toBeSwappedUp

            | _ -> failwith "Unexpected case in swapAndRemoveRightMost"
        
        // Find and remove the left most ancestor.
        let rec swapAndRemoveLeftMost = function
            // Look for element where left child is E.

            // Remove Red from Black - No need to rebalance
            | T(B,x,T(R,toBeSwappedUp,E,a),b) -> T(B,x,a,b),toBeSwappedUp
               
            // Remove Black with no children - Introduce sentinel
            | T(c',x,T(B,toBeSwappedUp,E,E),a) -> balance (trySwap' toBeSwappedUp) (T(c',x,S,a)),toBeSwappedUp

            // Remove Black with Red child from any parent (*parent not shown)
            | T(B,toBeSwappedUp,E,T(R,z,E,E)) -> T(B,z,E,E),toBeSwappedUp
            
            // Continue to the left most. 
            | T(c',x,T(c'',y,a,b),c) -> 
                let t,toBeSwappedUp = swapAndRemoveLeftMost (T(c'',y,a,b))
                let trySwap'' = trySwap' toBeSwappedUp
                balance trySwap'' (T(c',x,balance trySwap'' t,c)),toBeSwappedUp

            | _ -> failwith "Unexpected case in swapAndRemoveLeftMost"

        // Check one child left and it's right ancestors. 
        let swapAndRemoveLeftAdjacent = function
            // Remove Red from Black
            | T(B,x,T(R,toBeSwappedUp,a,E),b) -> T(B,x,a,b),toBeSwappedUp 
               
            // Remove Black with no children 
            | T(c',x,T(B,toBeSwappedUp,E,E),a) -> balance (trySwap' toBeSwappedUp) (T(c',x,S,a)),toBeSwappedUp
        
            // Continue to the right most. 
            | T(c',x,T(c'',y,a,b),c) -> 
                let t,toBeSwappedUp = swapAndRemoveRightMost (T(c'',y,a,b))
                let trySwap'' = trySwap' toBeSwappedUp
                balance trySwap'' (T(c',x,balance trySwap'' t,c)),toBeSwappedUp

            | _ -> failwith "Unexpected case in swapAndRemoveLeftAdjacent"
        // Check one child right and it's left ancestors. 
        let swapAndRemoveRightAdjacent = function

            // Remove Red from Black
            | T(B,x,a,T(R,toBeSwappedUp,E,b)) -> T(B,x,a,b),toBeSwappedUp
               
            // Remove Black with no children 
            | T(c',x,a,T(B,toBeSwappedUp,E,E)) -> balance(trySwap' toBeSwappedUp) (T(c',x,a,S)),toBeSwappedUp
        
            // Continue to the left most. 
            | T(c',x,a,T(c'',y,b,c)) -> 
                let t,toBeSwappedUp = swapAndRemoveLeftMost (T(c'',y,b,c))
                let trySwap'' = trySwap' toBeSwappedUp
                balance trySwap'' (T(c',x,a,balance trySwap'' t)),toBeSwappedUp

            | _ -> failwith "Unexpected case in swapAndRemoveRightAdjacent"

        let rec remove' tree =
            match tree with        
            // No children, remove the node. 
            | T(B,x,E,E) when x = element -> S
            | T(R,x,E,E) when x = element -> E

            // Swap and remove left adjacent
            | T(c',x,T(c'',y,a,b),c) when x = element -> 
                match swapAndRemoveLeftAdjacent tree with
                | (T(c''',z,d,e),toBeSwappedUp) ->
                    balance (trySwap' toBeSwappedUp) (T(c''',z,d,e))
                | x -> failwith "Unexpected non-tree element"

            // Swap and remove right adjacent
            | T(c',x,a,T(c'',y,b,c)) when x = element -> 
                match swapAndRemoveRightAdjacent tree with
                | (T(c''',z,d,e),toBeSwappedUp) ->
                    balance (trySwap' toBeSwappedUp) (T(c''',z,d,e))
                | x -> failwith "Unexpected non-tree element"
 
            // Node is not a match. 
            | T(c,e,a,b) -> 
                balance id (
                    if element < e then T(c,e,remove' a, b) 
                    else T(c,e,a,remove' b))

            | _ -> failwith "Attempting to remove from an empty sub-tree"

        root <| remove' tree

    let insert tree element =
        let rec insert' = function
            | E ->                          T(R,element,E,E)
            | T(color,y,a,b) ->
                if element < y then         balance id (T(color,y,insert' a,b))
                else if element > y then    balance id (T(color,y,a,insert' b))
                // Ignore inserting duplicates
                else                        T(color,y,a,b)
            | S -> failwith "Cannot insert Sentinel into tree."
        root <| insert' tree

    let print tree =
        let rec printit rb x =
            match rb with
            | T(c,y,a,b) -> 
                printfn "%s%d:%A" (String.replicate x "  ") y c
                printit a (x+1)
                printit b (x+1)
            | E -> printfn "%s%s" (String.replicate x "  ") "Empty"
            | S -> printfn "%s%s" (String.replicate x "  ") "Sentinel"
        printit tree 1

