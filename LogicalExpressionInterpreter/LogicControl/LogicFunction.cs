using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public class LogicFunction
    {
        private string Name = "";
        private string CombinedName = "";
        private string Expression = "";
        private List<Token> Tokens;
        private List<string> Operands;
        private string[,]? TruthTable;
        private List<LogicFunction> NestedFunctions;
        private List<string[]> FunctionsOperands;
        private DataDictionary<string, bool> ResultCollection;

        public LogicFunction(string name, string expression, string combinedName)
        {
            Name = name;
            CombinedName = combinedName;
            Expression = expression;
            Tokens = Tokenizer.Tokenize(expression);
            Operands = new List<string>();
            NestedFunctions = new List<LogicFunction>();
            FunctionsOperands = new List<string[]>();
            ResultCollection = new DataDictionary<string, bool>();
            SetOperandsFromExpression();
            FunctionsOperands.Add(Operands.ToArray());
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

        public string[,]? GetTruthTable()
        {
            return TruthTable;
        }

        public void AddNestedFunction(LogicFunction function)
        {
            NestedFunctions.Add(function);
            FunctionsOperands.Add(function.GetOperands().ToArray());
        }

        public List<LogicFunction> GetNestedFunctions()
        {
            return NestedFunctions;
        }

        public string GetNestedFunctionsNames()
        {
            string names = "";

            for (int i = 0; i < NestedFunctions.Count; i++)
            {
                names += NestedFunctions[i].Name + " ";
            }

            return names;
        }

        public string GetName()
        {
            return Name;
        }

        public string GetCombinedName()
        {
            return CombinedName;
        }

        public void AddResult(string values, bool result)
        {
            ResultCollection.Add(values, result);
        }

        public bool ContainsResult(string values)
        {
            return ResultCollection.ContainsKey(values);
        }

        public bool GetResult(string values)
        {
            return ResultCollection.GetValue(values);
        }
    }
}
