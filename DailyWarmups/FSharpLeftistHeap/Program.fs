// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Example
//[<EntryPoint>]
let main argv = 
    let x1 = Heap.insert Empty 21
    let x2 = Heap.insert x1 31
    let x3 = Heap.insert x2 42
    let x4 = Heap.insert x3 27    
    let x5 = Heap.insert x4 19        
    let x6 = Heap.insert x5 18            
    let x7 = Heap.insert x6 17

    let (v1, e1) = Heap.pop x7
    let (v2, e2) = Heap.pop v1
    let (v3, e3) = Heap.pop v2
    let (v4, e4) = Heap.pop v3
    let (v5, e5) = Heap.pop v4
    let (v6, e6) = Heap.pop v5
    let (v7, e7) = Heap.pop v6

    let l1 = List.toHeap [65; 64; 21; 89; 99; 83; 10; 82]

    let l2 = Heap.toList l1

    Heap.add 2 3 |> ignore
    printfn "%A" argv
    0 // return an integer exit code


