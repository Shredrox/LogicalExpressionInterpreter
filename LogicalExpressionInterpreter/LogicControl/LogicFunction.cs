using LogicalExpressionInterpreter.Parsing;

namespace LogicalExpressionInterpreter.LogicControl
{
    public class LogicFunction
    {
        private string Name = "";
        private readonly Guid ID = Guid.NewGuid();
        private string Expression = "";
        private List<Token> Tokens;
        private List<string> Operands;
        private string[,] TruthTable;
        private List<LogicFunction> ChildFunctions;

        public LogicFunction(string name, string expression)
        {
            Name = name;
            Expression = expression;
            Tokens = new List<Token>();
            Tokens = Tokenizer.Tokenize(expression);
            Operands = new List<string>();
            SetOperandsFromExpression();
        }

        private void SetOperandsFromExpression()
        {
            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].Type == Token.TokenType.LITERAL)
                {
                    Operands.Add(Tokens[i].Value);
                }
            }
        }

        public string GetExpression()
        {
            return Expression;
        }

        public List<Token> GetTokens()
        {
            return Tokens;
        }

        public List<string> GetOperands()
        {
            return Operands;
        }

        public void SetTruthTable(string[,] truthTable)
        {
            TruthTable = truthTable;
        }

        public string[,] GetTruthTable()
        {
            return TruthTable;
        }

        public void AddChildFunction(LogicFunction function)
        {
            ChildFunctions.Add(function);
        }

        public string GetName()
        {
            return Name;
        }
    }
}
