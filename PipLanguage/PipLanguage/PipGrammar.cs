using Irony.Parsing;
namespace PipLanguage
{
    [Language("PipLangProof", "1.0", "Simplified language to get started.")]
    public class PipGrammar : Grammar {
        public static readonly PipGrammar Instance;

        // 2. Non-Terminals
        public static readonly NonTerminal function = new NonTerminal("function");
        public static readonly NonTerminal function_header = new NonTerminal("function_header");
        public static readonly NonTerminal function_body = new NonTerminal("function_body");
        public static readonly NonTerminal function_parameter_list_opt = new NonTerminal("function_parameter_list_opt");
        public static readonly NonTerminal function_parameter = new NonTerminal("function_parameter");
        public static readonly NonTerminal statement_list_opt = new NonTerminal("statement+");
        public static readonly NonTerminal statement = new NonTerminal("statement");
        public static readonly NonTerminal integral_type = new NonTerminal("integral_type");

        // 2. Non-terminals
        public static readonly NonTerminal Expr = new NonTerminal("Expr");
        public static readonly NonTerminal Term = new NonTerminal("Term");
        public static readonly NonTerminal BinExpr = new NonTerminal("BinExpr");
        public static readonly NonTerminal ParExpr = new NonTerminal("ParExpr");
        public static readonly NonTerminal UnExpr = new NonTerminal("UnExpr");
        public static readonly NonTerminal UnOp = new NonTerminal("UnOp");
        public static readonly NonTerminal BinOp = new NonTerminal("BinOp", "operator");
        public static readonly NonTerminal PostFixExpr = new NonTerminal("PostFixExpr");
        public static readonly NonTerminal PostFixOp = new NonTerminal("PostFixOp");
        public static readonly NonTerminal AssignmentStmt = new NonTerminal("AssignmentStmt");
        public static readonly NonTerminal AssignmentOp = new NonTerminal("AssignmentOp");
        public static readonly NonTerminal PropertyAccess = new NonTerminal("PropertyAccess");
        public static readonly NonTerminal FunctionCall = new NonTerminal("FunctionCall");

        public static readonly NumberLiteral number = new NumberLiteral("number");
        public static readonly IdentifierTerminal identifier = new IdentifierTerminal("identifier");
        public static readonly KeyTerm functionKeyWord = new KeyTerm("function", "FunctionKeyWord");

        public static readonly KeyTerm lbr = new KeyTerm("{", "lbr");
        public static readonly KeyTerm rbr = new KeyTerm("}", "rbr");
        public static readonly KeyTerm lpar = new KeyTerm("(", "lpar");
        public static readonly KeyTerm rpar = new KeyTerm(")", "rpar");
        public static readonly KeyTerm dot = new KeyTerm(".", "dot");
        public static readonly KeyTerm comma = new KeyTerm(",", "comma");
        public static readonly KeyTerm colon = new KeyTerm(":", "colon");
        public static readonly KeyTerm semicolon = new KeyTerm(";", "semicolon");

        static PipGrammar() {
            Instance = new PipGrammar();
        }

        protected PipGrammar() {
            this.GrammarComments = @"Simplified version of the Shelby language to get started.";

            // 1. Terminals
            
            function.Rule = function_header + function_body;
            
            // TODO: Integrate return type
            function_header.Rule = functionKeyWord + identifier + lpar + function_parameter_list_opt + rpar + colon + identifier;

            function_parameter_list_opt.Rule = MakeStarRule(function_parameter_list_opt, comma, function_parameter);
            function_parameter.Rule = identifier + colon + integral_type;
            function_body.Rule = lbr + statement_list_opt + rbr;

            statement_list_opt.Rule = MakeStarRule(statement_list_opt, statement);
            statement.Rule = Empty | semicolon | BinExpr + semicolon;

            integral_type.Rule = ToTerm("int");

            // 3. BNF rules
            Expr.Rule = BinExpr | Term | UnExpr | FunctionCall | PropertyAccess;
            Term.Rule = number | ParExpr | identifier;
            ParExpr.Rule = "(" + Expr + ")";
            UnExpr.Rule = UnOp + Term;
            UnOp.Rule = ToTerm("+") | "-" | "++" | "--";
            BinExpr.Rule = Expr + BinOp + Expr;
            BinOp.Rule = ToTerm("+") | "-" | "*" | "/" | "^";
            PropertyAccess.Rule = identifier + "." + identifier;
            FunctionCall.Rule = identifier + ParExpr;

            // 4. Operators precedence
            RegisterOperators(1, "+", "-");
            RegisterOperators(2, "*", "/");
            RegisterOperators(3, Associativity.Right, "^");

            MarkPunctuation("(", ")", ".");
            MarkTransient(Term, Expr, BinOp, UnOp, AssignmentOp, ParExpr);

            this.Root = function;
        }
    }
}
