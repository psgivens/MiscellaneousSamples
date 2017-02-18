// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open PipCompiler
open System.Reflection
open System

[<EntryPoint>]
let main argv = 

    use reader = new System.IO.StreamReader (Assembly.GetExecutingAssembly().GetManifestResourceStream("Sample1.source"))
    let t =  
        reader.ReadToEnd()
        |> ILCompiler.Function
    ILCompiler.Save()
    
    let ``method`` = t.GetMethod ("Doit", BindingFlags.Public ||| BindingFlags.Instance)
    let t3i = Activator.CreateInstance t
    let result = ``method``.Invoke(t3i, ([| 5; 6 |] ))
    System.Diagnostics.Debug.Assert(result.Equals(37))
 
    Console.WriteLine("Finished without throwing exception.")
    Console.ReadLine() |> ignore
    printfn "%A" argv
    0 // return an integer exit code
