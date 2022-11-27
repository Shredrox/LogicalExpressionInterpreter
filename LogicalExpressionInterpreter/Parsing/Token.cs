namespace LogicalExpressionInterpreter.Parsing
{
    public class Token
    {
        public TokenType Type;
        public string Value = "";

        public enum TokenType
        {
            AND,
            OR,
            NOT,
            LITERAL,
            OPEN_PAREN,
            CLOSE_PAREN,
            NESTED_FUNCTION
        }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
