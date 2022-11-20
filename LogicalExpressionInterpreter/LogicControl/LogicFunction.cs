using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public class LogicFunction
    {
        private string Expression = "";
        private List<Token> Tokens;
        private List<string> Operands;
        private string[,] TruthTable;

        public LogicFunction(string expression)
        {
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
    }
}
