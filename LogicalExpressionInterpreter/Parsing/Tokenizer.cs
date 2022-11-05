using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.Parsing
{
    public static class Tokenizer
    {
        public static DynamicArray<Token> Tokenize(string input)
        {
            DynamicArray<Token> tokens = new();

            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case ' ': break;
                    case '&': tokens.Add(new Token(Token.TokenType.AND, "&&")); i++; break;
                    case '|': tokens.Add(new Token(Token.TokenType.OR, "||")); i++; break;
                    case '(': tokens.Add(new Token(Token.TokenType.OPEN_PAREN, "(")); break;
                    case ')': tokens.Add(new Token(Token.TokenType.CLOSE_PAREN, ")")); break;
                    case '!': tokens.Add(new Token(Token.TokenType.NOT, "!")); break;
                    default: tokens.Add(new Token(Token.TokenType.LITERAL, input[i].ToString())); break;
                }
            }

            return tokens;
        }
    }
}
