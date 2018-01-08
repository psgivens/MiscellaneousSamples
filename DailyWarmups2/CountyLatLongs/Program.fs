// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open DoWork
//open Generations

[<EntryPoint>]
let main argv = 
//    generateAll ()
    //ingestLocations ()
    //ingestRestaurants ()
    generateProximalJs "PR0015844"
    generateProximalJs "PR0023624"
    generateProximalJs "PR0014966"
    generateProximalJs "PR0002740"
    generateProximalJs "PR0144538"
    generateProximalJs "PR0008522"
    generateProximalJs "PR0153036"
    generateProximalJs "PR0020793"
    generateProximalJs "PR0181184"
    generateProximalJs "PR0030485"
    generateProximalJs "PR0148088"
    generateProximalJs "PR0147425"
    generateProximalJs "PR0187091"
    generateProximalJs "PR0187077"
    generateProximalJs "PR0192486"
    generateProximalJs "PR0189708"
    generateProximalJs "PR0006335"
    generateProximalJs "PR0160724"
    generateProximalJs "PR0188528"
    generateProximalJs "PR0043395"
    generateProximalJs "PR0161456"
    generateProximalJs "PR0038551"
    generateProximalJs "PR0006818"
    generateProximalJs "PR0196651"
    generateProximalJs "PR0147588"
    printfn "%A" argv
    0 // return an integer exit code
