namespace PipCompiler

open Irony.Parsing
open System
open PipLanguage
open AbstractSyntaxTree

module ParseTreeToAst =

    let private not_null (term:BnfTerm) (node:ParseTreeNode) = 
        if (node.Term.Name <> term.Name && node.Term.Name.TrimEnd('+') <> term.Name)
            then invalidArg "node" (sprintf "Node %s expected but found %s." term.Name node.Term.Name)
        node    

    let private (|Expression|_|) f (node:ParseTreeNode) =
        Some(f node)

    let private (|TokenType|_|) (term:BnfTerm) (node:ParseTreeNode) =
        match node with
        | n when n.Term.Name.Equals term.Name -> Some(node)
        | _ -> None
                
    let private (|TokenText|_|) (term:BnfTerm) (node:ParseTreeNode) = 
        match node with
        | TokenType term _ -> Some(node.Token.Text)
        | _ -> None

    let private (|TokenValue|_|) (value:string) (node:ParseTreeNode) = 
        match node.Term.Name with
        | value -> Some(node) 
        | _ -> None

    let private child_list (term:BnfTerm) (node:ParseTreeNode) =
        node 
        |> not_null term
        |> fun n -> n.ChildNodes
        |> Seq.toList

    (* Begin transversing the tree. *)

    let private binary_operator (node:ParseTreeNode) :Binary_Operator=
        {span=node.Span; operator=(string node.Token.Value)}
    
    let private integral_type (node:ParseTreeNode) :Integral_Type = 
        match node.Term.Name with
        | "integral_type" -> 
            match node |> child_list PipGrammar.integral_type with 
                | TokenValue "short" _ ::[]->    Integral_Type.Int16
                | TokenValue "int" _ ::[]->      Integral_Type.Int32
                | TokenValue "long" _ ::[]->     Integral_Type.Int64
                | node::[] -> failwith (sprintf "unrecognized integral type %s" node.Term.Name)
                | _ -> failwith "incorrect child count of integral type"
        | _ as name -> failwith (sprintf "unrecognized integral type %s" name)

    let rec private expression (node:ParseTreeNode) :Expression_Node =
        {   span=node.Span; 
            expression=
                match node.Term.Name with
                | "statement" ->    (expression node.ChildNodes.[0]).expression
                | "BinExpr" ->      binary_expression node
                | "identifier" ->   Term(Identifier node.Token.Text)
                | "number" ->       Term(Number(Convert.ToInt32(node.Token.Value)))
                | "FunctionCall" -> Empty  // Not yet implemented.
                | _ as name ->      failwith (sprintf "Unexpected node %s" name)}

    and private binary_expression (node:ParseTreeNode) :Expression =
        match node |> child_list PipGrammar.BinExpr with
        |   Expression expression left::
            Expression binary_operator operator::
            Expression expression right::
            [] ->       BinaryExpression(left, operator, right) 
        | _ ->          failwith "binary expression has wrong children"
    
    (* Why do we have a "statement+" node inside of another "statement+" node? *)
    let private statement (node:ParseTreeNode) :Expression_Node =
        match node |> child_list PipGrammar.statement_list_opt
            with
            | only::[] -> expression only
            | _ ->          failwith ("statement has incorrect children")   
    
    let private function_body (node:ParseTreeNode) :Function_Body =
        match node |> child_list PipGrammar.function_body 
            with
            |   TokenType PipGrammar.lbr _::
                TokenType PipGrammar.statement_list_opt opt::
                TokenType PipGrammar.rbr _::
                [] -> 
                { span=node.Span;
                  value=
                    opt 
                    |> child_list PipGrammar.statement_list_opt 
                    |> List.map statement }
            | _ -> failwith "expected { + statement_list_opt + }"

    let private function_parameter (node:ParseTreeNode) :Function_Parameter =
        match node |> child_list PipGrammar.function_parameter
            with
            | TokenText PipGrammar.identifier name::
              TokenType PipGrammar.colon _::
              Expression integral_type t::
              tail ->   {span=node.Span; name=name; ofType=t}
            | _ ->      failwith ("function_parameter has incorrect children")
    
    let private function_parameters (node:ParseTreeNode) :Function_Parameters =
        {   span = node.Span;
            value = 
                node
                |> child_list PipGrammar.function_parameter_list_opt 
                |> List.map function_parameter }

    let private function_header (node:ParseTreeNode) :Function_Header =
        match node |> child_list PipGrammar.function_header
            with
            |   TokenType PipGrammar.functionKeyWord _::
                TokenText PipGrammar.identifier id::
                TokenType PipGrammar.lpar _::
                TokenType PipGrammar.function_parameter_list_opt parameters::
                TokenType PipGrammar.rpar _::
                TokenType PipGrammar.colon _::
                TokenText PipGrammar.identifier returnType::
                [] -> 
                {span=node.Span; name=id; parameters=function_parameters parameters; returnType=returnType}
            | _ -> failwith "Expected function header"

    let extract (node:ParseTreeNode) :Function = 
        match node |> child_list PipGrammar.``function``
            with
            |   TokenType PipGrammar.function_header header::
                TokenType PipGrammar.function_body body::
                [] -> 
                {span=node.Span; header=function_header header; body=function_body body}
            | _ -> failwith "Expected function header"



