namespace PipCompiler

open System
open System.Reflection
open System.Diagnostics.SymbolStore
open System.Reflection.Emit
open Irony.Parsing
open PipLanguage


type ILCompiler () = 
    
    static let aName = new AssemblyName("_GeneratedAssembly")
    static let ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave)
    
    static member private GetTypeBuilder fileName =
        let mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll", true)
        let writer = 
            if String.IsNullOrWhiteSpace fileName 
                then new AstToIL.NullWriter() :> ISymbolDocumentWriter 
                else mb.DefineDocument(fileName, SymDocumentType.Text, SymLanguageType.ILAssembly, SymLanguageVendor.Microsoft)
        let tb = mb.DefineType ("User", TypeAttributes.Public)
        tb, writer
    
    static member CreateFunction fileName =   
        let functionText = System.IO.File.ReadAllText fileName
        let typeBuilder, writer = ILCompiler.GetTypeBuilder fileName
        ILCompiler.CreateFunction (functionText, typeBuilder, writer)

    static member Function functionText =
        let typeBuilder, writer = ILCompiler.GetTypeBuilder ""
        ILCompiler.CreateFunction (functionText, typeBuilder, writer)

    static member CreateFunction (functionText, typeBuilder:TypeBuilder, writer:ISymbolDocumentWriter) =   
        (* 1. Parse *)
        let parserInstance = new Parser(PipGrammar.Instance);
        let parseTree = parserInstance.Parse functionText
        
        (* 2. Transform *)
        let functionNode = ParseTreeToAst.extract parseTree.Root

        (* 3. Build *)
        AstToIL.build (typeBuilder, writer, functionNode)
        
        (* 4. Create type *)
        typeBuilder.CreateType()        
    
    static member Save () = 
        ab.Save(aName.Name + ".dll")


