namespace PipCompiler

module AstToIL =

    open System
    open System.Collections.Generic
    open System.Diagnostics.SymbolStore
    open System.Reflection
    open System.Reflection.Emit
    open AbstractSyntaxTree

    
    type NullWriter () =
        interface ISymbolDocumentWriter with
            override this.SetSource source = ()
            override t.SetCheckSum (x,y) = ()

    type private FunctionParameter(name:string, ``type``:Type, position:int) = 
        member val Name = name with get
        member val Type = ``type`` with get
        member val Position = position with get

    type private IlBinder () = 
        let mutable nextParamPosition = 0
        let parameters = new Dictionary<string, FunctionParameter>()
    
        member this.Parameters 
            with get() = parameters.Values |> Seq.cast<FunctionParameter>
        
        member this.RegisterParameter (name, ``type``) =
            let param = new FunctionParameter (name, ``type``, nextParamPosition)
            parameters.Add(name, param)
            nextParamPosition <- nextParamPosition + 1 
            param

        member this.ResolveParameter parameterName = 
            match (parameters.TryGetValue( parameterName)) with
            | (true, parameter) -> parameter
            | _                 -> invalidArg "parameterName" (sprintf "parameter %s not found" parameterName)
    
        member this.ResolveMethod methodName =
            Operators.typeof<Math>.GetMethods()
            |> Seq.cast<Type>
            |> Seq.tryFind(fun m -> m.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase))

        
    let private declareParameters (parameters:list<Function_Parameter>) =            
        let binder = new IlBinder()
        binder, parameters 
            |>  List.map (fun parameter -> 
                binder.RegisterParameter (parameter.name, (* parameter.ofType *) Operators.typeof<int>))

    let private resolveType typeName =
        match typeName with 
        | "int" ->  Operators.typeof<int>
        | _ ->      failwith (sprintf "type %s not found" typeName)

    let private declareMethod (typeBuilder:TypeBuilder, node:Function_Header) :(IlBinder * MethodBuilder) =
        let name = node.name
        let binder, pnodes =  declareParameters(node.parameters.value) 
        let parameters = 
            pnodes
            |> Seq.map (fun p -> p.Type)
            |> Seq.toArray                
        binder, typeBuilder.DefineMethod(
            name,
            MethodAttributes.Public,
            CallingConventions.Standard ||| CallingConventions.HasThis,
            resolveType node.returnType,
            parameters);

    type private FunctionBuilder (_builder:TypeBuilder, _writer:ISymbolDocumentWriter, _il:ILGenerator, _binder:IlBinder) =
              
        member private this.build (number:int) = 
            _il.Emit(OpCodes.Ldc_I4, number)

        member private this.build (op:Binary_Operator) =
            match op.operator with
            | "+" ->    _il.Emit OpCodes.Add
            | "-" ->    _il.Emit OpCodes.Sub
            | "*" ->    _il.Emit OpCodes.Mul
            | "/" ->    _il.Emit OpCodes.Div
            | _ ->      failwith (sprintf "unrecognized operator found: %s" op.operator)

        member private this.build (node:Expression_Node) =
            match node.expression with
            | BinaryExpression(l, o, r) -> 
                this.build l
                this.build r
                this.build o
            | Term(Identifier(identifier)) -> 
                let p = _binder.ResolveParameter(identifier)
                let position = int16 (p.Position + 1)
                match position with
                    | 0s -> _il.Emit OpCodes.Ldarg_0
                    | 1s -> _il.Emit OpCodes.Ldarg_1
                    | 2s -> _il.Emit OpCodes.Ldarg_2
                    | 3s -> _il.Emit OpCodes.Ldarg_3
                    | _ ->  _il.Emit (OpCodes.Ldarg_S, position)
            | Term(Number(number)) -> this.build number            
//            | "FunctionCall" -> () // Not yet implemented.
            | _ -> ()

        member private this.MarkSequencePoint (span:SourceSpan) =        
            let start = span.Location
            match _writer with
            | :? NullWriter as w -> ()
            | _ -> _il.MarkSequencePoint(_writer, start.Line + 1, start.Column, start.Line + 1, start.Column + span.Length)

        member this.build (body:Function_Body) = 
            let binExpr = body.value.Head
            this.MarkSequencePoint binExpr.span
            this.build binExpr
            
    let build (typeBuilder:TypeBuilder, writer:ISymbolDocumentWriter, node:Function) =             
        (* 1. Build declaration *)
        let binder, ``method`` = declareMethod (typeBuilder, node.header)
        binder.Parameters
        |> Seq.iter(fun p ->
            ``method``.DefineParameter(p.Position + 1, ParameterAttributes.None, p.Name)
            |> ignore)

        (* 2. Generate IL *)
        let il = ``method``.GetILGenerator()
        let builder = new FunctionBuilder (typeBuilder, writer, il, binder)
        builder.build node.body
        il.Emit(OpCodes.Ret)

