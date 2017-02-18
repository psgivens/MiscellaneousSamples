namespace PipCompiler

module AbstractSyntaxTree = 
    type SourceSpan = Irony.Parsing.SourceSpan

    type Function =                 {span:SourceSpan; header:Function_Header; body: Function_Body}
    and Function_Header =           {span:SourceSpan; name:Identifier; parameters:Function_Parameters; returnType:Type_Definition}
    and Function_Parameters =       {span:SourceSpan; value:list<Function_Parameter>}
    and Function_Parameter =        {span:SourceSpan; name:Identifier; ofType:Integral_Type}
    and Function_Body =             {span:SourceSpan; value:list<Expression_Node>}    
    and Binary_Operator =           {span:SourceSpan; operator:string}
    and Expression_Node =           {span:SourceSpan; expression:Expression}
    and Unary_Operator =            {span:SourceSpan; operator:string}
    and Member_Definition =         {span:SourceSpan; InstancName:Identifier; PropertyName:Identifier }
    and FunctionCall_Parameters =   {span:SourceSpan; parameters:list<Expression>}    
    and Type_Definition =           string
    and Identifier =                string
    and Integral_Type =              
        | Int16
        | Int32
        | Int64
    and Term =
        | Number of int
        | ParExpr of Expression_Node
        | Identifier of Identifier    
    and Expression = 
        | Empty // Not valid, but allows value while developing
        | BinaryExpression  of Expression_Node * Binary_Operator * Expression_Node
        | Term              of Term
        | UniraryExpression of Expression_Node * Unary_Operator * Expression_Node
        | FunctionCall      of Member_Definition * FunctionCall_Parameters
        | PropertyAccess    of Member_Definition

