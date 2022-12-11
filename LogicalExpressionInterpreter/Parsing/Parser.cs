using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.Parsing
{
    public static class Parser
    {
        private static int CheckOperatorPriority(Token token)
        {
            switch (token.Type)
            {
                case Token.TokenType.NOT: return 3;
                case Token.TokenType.AND: return 2;
                case Token.TokenType.OR: return 1;
            }

            return 0;
        }

        public static List<Token> ConvertToPostfix(List<Token> tokens)
        {
            ObjectStack<Token> stack = new();
            List<Token> postfixExpression = new();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == Token.TokenType.LITERAL || tokens[i].Type == Token.TokenType.NESTED_FUNCTION)
                {
                    postfixExpression.Add(tokens[i]);
                }
                else if (tokens[i].Type == Token.TokenType.OPEN_PAREN)
                {
                    stack.Push(tokens[i]);
                }
                else if (tokens[i].Type == Token.TokenType.CLOSE_PAREN)
                {
                    while (stack.Count() > 0 && stack.Peek().Type != Token.TokenType.OPEN_PAREN)
                    {
                        postfixExpression.Add(stack.Pop());
                    }

                    stack.Pop();
                }
                else if (tokens[i].Type == Token.TokenType.NOT
                    || tokens[i].Type == Token.TokenType.AND
                    || tokens[i].Type == Token.TokenType.OR)
                {
                    while (stack.Count() > 0 && CheckOperatorPriority(stack.Peek()) >= CheckOperatorPriority(tokens[i]))
                    {
                        postfixExpression.Add(stack.Pop());
                    }

                    stack.Push(tokens[i]);
                }
            }

            while (stack.Count() > 0)
            {
                postfixExpression.Add(stack.Pop());
            }

            return postfixExpression;
        }

        public static string ConvertToInfix(string expression)
        {
            Stack<string> stack = new();
            string[] splitExpression = expression.Split(" ");
            string output = "";

            for (int i = 0; i < splitExpression.Length; i++)
            {
                if (char.IsLetter(splitExpression[i][0]))
                {
                    stack.Push(splitExpression[i]);
                }
                else
                {
                    string operand1 = stack.Pop();
                    string operand2 = stack.Pop();
                    output = operand2 + " " + splitExpression[i] + " " + operand1;
                    stack.Push(output);
                }
            }

            return output;
        }
    }
}
