using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.Parsing
{
    public static class Tokenizer
    {
        public static List<Token> Tokenize(string input)
        {
            var split = Utility.Split(input, ' ');
            List<Token> tokens = new();

            for (int l = 0; l < split.Length; l++)
            {
                var line = split[l];

                for (int i = 0; i < line.Length; i++)
                {
                    if (Utility.ContainsMoreThanOneLetter(line))
                    {
                        if (Utility.GetCountOf(line, ')') > 1)
                        {
                            tokens.Add(new Token(Token.TokenType.NESTED_FUNCTION, Utility.TrimEndToPenultimate(line, ')')));
                            var trimValue = Utility.TrimEndToPenultimateValue(line, ')');

                            for (int k = 0; k < trimValue.Length; k++)
                            {
                                tokens.Add(new Token(Token.TokenType.CLOSE_PAREN, ")"));
                            }
                            break;
                        }
                        else if (Utility.GetCountOf(line, '(') > 1)
                        {
                            var trimValue = Utility.TrimStartValue(line, '(');
                            for (int k = 0; k < trimValue.Length; k++)
                            {
                                tokens.Add(new Token(Token.TokenType.OPEN_PAREN, "("));
                            }
                            tokens.Add(new Token(Token.TokenType.NESTED_FUNCTION, Utility.TrimStart(line, '(')));
                            break;
                        }

                        tokens.Add(new Token(Token.TokenType.NESTED_FUNCTION, Utility.TrimEndToPenultimate(line,')')));
                        break;
                    }

                    switch (line[i])
                    {
                        case ' ': break;
                        case '&': tokens.Add(new Token(Token.TokenType.AND, "&&")); i++; break;
                        case '|': tokens.Add(new Token(Token.TokenType.OR, "||")); i++; break;
                        case '(': tokens.Add(new Token(Token.TokenType.OPEN_PAREN, "(")); break;
                        case ')': tokens.Add(new Token(Token.TokenType.CLOSE_PAREN, ")")); break;
                        case '!': tokens.Add(new Token(Token.TokenType.NOT, "!")); break;
                        default: tokens.Add(new Token(Token.TokenType.LITERAL, line[i].ToString())); break;
                    }
                }
            }

            return tokens;
        }
    }
}
