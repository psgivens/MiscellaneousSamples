// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

// return an integer exit code
module  Program

open rbtree

[<EntryPoint>]
let main argv = 
    let v = RedBlackTree.insert E 42
    let t = [1..12] |> Seq.fold (fun acc x -> RedBlackTree.insert acc x) v
    RedBlackTree.print t
    RedBlackTree.validate t 
    let u = t |> RedBlackTree.remove 8 |> RedBlackTree.print
    let w = RedBlackTree.remove 6 t
    RedBlackTree.print w
    RedBlackTree.validate w 
    let x = RedBlackTree.remove 4 w
    RedBlackTree.print x
    RedBlackTree.validate x 

    // Leaves an unbalanced tree. Fix it. 
    let v = RedBlackTree.remove 3 x
    RedBlackTree.print v
    RedBlackTree.validate v 

    let y = RedBlackTree.remove 4 t
    RedBlackTree.validate y 
    let z = RedBlackTree.remove 3 t 
    RedBlackTree.validate z 
    RedBlackTree.print t
    printfn "%A" (RedBlackTree.print t)    
    0
