using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public class LogicFunction
    {
        private string Expression = "";
        private DynamicArray<Token> Tokens;
        private DynamicArray<string> Operands;

        public LogicFunction(string expression)
        {
            Expression = expression;
            Tokens = new DynamicArray<Token>();
            Tokens = Tokenizer.Tokenize(expression);
            Operands = new DynamicArray<string>();
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

        public DynamicArray<Token> GetTokens()
        {
            return Tokens;
        }

        public DynamicArray<string> GetOperands()
        {
            return Operands;
        }

    }
}
