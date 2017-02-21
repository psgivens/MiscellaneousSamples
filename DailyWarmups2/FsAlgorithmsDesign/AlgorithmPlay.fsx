


//let path = 
//    String.concat 
//        "" [ __SOURCE_DIRECTORY__ ; @"..\packages\FSharp.Charting.0.90.14\lib\net40\FSharp.Charting.dll"]

#r @"..\packages\FSharp.Charting.0.90.14\lib\net40\FSharp.Charting.dll"

open FSharp.Charting

let limit = 20
let lower = 11

let firstLine x = 
    let y = lower
    let z = y*y*y
    x, x*x*x + z

let secondLine x =
    let y = x - 1
    let z = x - 2
    x, y*y*y + z*z*z 

let chart = Chart.Combine (
    [Chart.Line([ for x in 3 .. limit -> firstLine x])
     Chart.Line([ for x in 3 .. limit -> secondLine x])
     ])
chart.ShowChart()


