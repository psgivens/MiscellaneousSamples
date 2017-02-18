namespace PipCompiler.Tests


open NUnit.Framework
open FsUnit
open PipCompiler
open System.Reflection
open System

module UnitTests =
    open PipCompiler
    
    [<Test>]
    let ``Simple math function with order of operations`` () =
        let source = "function Doit(i:int, j:int) :int
{                            
    (2 + i) * j - i;                    
}"
        let t = ILCompiler.Function(source);
        ILCompiler.Save()
            
        let t3i = Activator.CreateInstance t
        let ``method`` = t.GetMethod ("Doit", BindingFlags.Public ||| BindingFlags.Instance)

        ``method``.Invoke(t3i, ([| 5; 6 |] )) 
        |> should equal 37
    