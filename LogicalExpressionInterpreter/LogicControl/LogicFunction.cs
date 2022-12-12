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
        private DataDictionary<string, bool> ResultCollection;

        public LogicFunction(string name, string expression, string combinedName)
        {
            Name = name;
            CombinedName = combinedName;
            Expression = expression;
            Tokens = Tokenizer.Tokenize(expression);
            Operands = new List<string>();
            NestedFunctions = new List<LogicFunction>();
            ResultCollection = new DataDictionary<string, bool>();
            SetOperandsFromExpression();
        }

        public LogicFunction(string name, string expression, string combinedName, List<Token> tokens)
        {
            Name = name;
            CombinedName = combinedName;
            Expression = expression;
            Tokens = tokens;
            Operands = new List<string>();
            NestedFunctions = new List<LogicFunction>();
            ResultCollection = new DataDictionary<string, bool>();
            SetOperandsFromExpression();
        }

        private void SetOperandsFromExpression()
        {
            string[] splitName = Utility.Split(CombinedName, '(');
            string[] operands = Utility.Split(Utility.TrimEnd(splitName[1], ')'), ',');

            for (int i = 0; i < operands.Length; i++)
            {
                Operands.Add(operands[i]);
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

        public void SetNestedFunctions(List<LogicFunction> nestedFunctions)
        {
            NestedFunctions = nestedFunctions;
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
