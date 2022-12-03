using LogicalExpressionInterpreter.BinaryTree;
using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public static class LogicController
    {
        private static List<LogicFunction> userFunctions = new();
        //private static string path = "../../UserFunctions.txt";
        private static string path = "../../../../LogicalExpressionInterpreter/bin/UserFunctions.txt";

        public static void SaveFunctions()
        {
            DataControl.SaveToFile(userFunctions, path);
        }

        public static void LoadFunctions()
        {
            if(File.Exists(path))
            {
                userFunctions = DataControl.LoadFromFile(path);
            }
        }

        public static void SetUserFunctions(List<LogicFunction> functions)
        {
            userFunctions = functions;
        }

        public static bool FunctionExists(string name)
        {
            for (int i = 0; i < userFunctions.Count; i++)
            {
                if (userFunctions[i].GetName() == name)
                {
                    return true;
                }
            }

            return false;
        }

        public static int GetFunctionCount()
        {
            return userFunctions.Count;
        }

        public static void AddUserFunction(LogicFunction function)
        {
            userFunctions.Add(function);
        }

        public static List<string> GetFunctionsInfo()
        {
            List<string> functionsInfo = new List<string>();
            string line;

            for (int i = 0; i < userFunctions.Count; i++)
            {
                line = i + 1 + ". " + userFunctions[i].GetName() + ": " + userFunctions[i].GetExpression();
                if (userFunctions[i].GetNestedFunctions().Count != 0)
                {
                    line = i + 1 + ". "
                        + userFunctions[i].GetName() + ": "
                        + userFunctions[i].GetNestedFunctionsNames()
                        + userFunctions[i].GetExpression();
                }
                functionsInfo.Add(line);
            }

            return functionsInfo;
        }

        public static void Run()
        {
            if (File.Exists(path))
            {
                userFunctions = DataControl.LoadFromFile(path);
            }

            while (true)
            {
                Console.WriteLine("Enter command: ");

                string input = "0";
                int lineCounter = 1;
                List<string> inputLines = new();
                while (input != "" && input != " ")
                {
                    input = Console.ReadLine();
                    if (input == "" || input == " ")
                    {
                        continue;
                    }
                    if (Utility.ToUpper(Utility.TrimStart(Utility.Split(input, ' ')[0], ' '))!= "FIND" && (lineCounter == 1))
                    {
                        inputLines.Add(input);
                        break;
                    }
                    inputLines.Add(input);
                    lineCounter++;
                }

                for (int i = 0; i < inputLines.Count; i++)
                {
                    inputLines[i] = Utility.TrimStart(inputLines[i], ' ');
                }
                var inputSplit = Utility.Split(inputLines[0], ' ', 2);

                if (Utility.StringIsNullOrEmpty(inputSplit[1]) 
                    && Utility.ToUpper(inputSplit[0]) != "PRINTALL"
                    && Utility.ToUpper(inputSplit[0]) != "EXIT")
                {
                    Console.WriteLine("Invalid Command.");
                    continue;
                }

                switch (Utility.ToUpper(inputSplit[0]))
                {
                    case "DEFINE": AddFunction(inputSplit[1]); DataControl.SaveToFile(userFunctions, path); break;
                    case "REMOVE": RemoveFunction(inputSplit[1]); DataControl.SaveToFile(userFunctions, path); break;
                    case "PRINTALL": PrintFunctions(); break;
                    case "SOLVE": SolveFunction(inputSplit[1]); break;
                    case "ALL": CreateTruthTable(inputSplit[1]); break;
                    case "FIND": FindFunction(inputLines); break;
                    case "EXIT": DataControl.SaveToFile(userFunctions, path); return;
                    default: Console.WriteLine("Invalid Command."); break;
                }

                Console.WriteLine();
            }
        }

        public static void AddFunction(string input)
        {
            string[] inputSplit = Utility.Split(input, ':');
            string[] splitName = Utility.Split(inputSplit[0], '(');
            string name = splitName[0];

            var tokens = Tokenizer.Tokenize(inputSplit[1]);

            for (int i = 0; i < userFunctions.Count; i++)
            {
                if (userFunctions[i].GetName() == name) 
                {
                    Console.WriteLine("A function with this name already exists.");
                    return;
                }
            }

            string[] operands = Utility.Split(Utility.TrimEnd(splitName[1], ')'), ',');
            string expression = Utility.TrimStart(inputSplit[1], ' ');
            string[] nestedFunctionOperands;
            string nestedFunctionName = "";

            bool hasNestedFunction = false;
            int literalCount = 0;
            List<LogicFunction> nestedFunctions = new();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == Token.TokenType.NESTED_FUNCTION)
                {
                    hasNestedFunction = true;
                    var spl = Utility.Split(tokens[i].Value, '(');
                    nestedFunctionName = spl[0];
                    nestedFunctionOperands = Utility.Split(Utility.TrimEnd(spl[1], ')'), ',');
                    literalCount += nestedFunctionOperands.Length;

                    for (int k = 0; k < userFunctions.Count; k++)
                    {
                        if (userFunctions[k].GetName() == nestedFunctionName)
                        {
                            nestedFunctions.Add(userFunctions[k]);
                        }
                    }
                }
            }

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == Token.TokenType.LITERAL)
                {
                    literalCount++;
                }
            }
            
            if (literalCount != operands.Length)
            {
                Console.WriteLine("Invalid number of operands.");
                return;
            }
            else if (hasNestedFunction && nestedFunctions.Count == 0)
            {
                Console.WriteLine("Nested Function: " + nestedFunctionName + " doesn't exist.");
                return;
            }

            var newFunction = new LogicFunction(name, expression, inputSplit[0], tokens);
            newFunction.SetNestedFunctions(nestedFunctions);
            userFunctions.Add(newFunction);
        }

        public static void RemoveFunction(string input)
        {
            for (int i = 0; i < userFunctions.Count; i++)
            {
                if (userFunctions[i].GetName() == input)
                {
                    userFunctions.RemoveAt(i);
                }
            }
        }

        public static void PrintFunctions()
        {
            if (userFunctions.Count == 0)
            {
                Console.WriteLine("No added functions.");
                return;
            }

            Console.WriteLine("Current Functions: ");

            string line;

            for (int i = 0; i < userFunctions.Count; i++)
            {
                line = i + 1 + ". " + userFunctions[i].GetName() + ": " + userFunctions[i].GetExpression();
                if (userFunctions[i].GetNestedFunctions().Count != 0)
                {
                    line = i + 1 + ". " 
                        + userFunctions[i].GetName() + ": " 
                        + userFunctions[i].GetNestedFunctionsNames() 
                        + userFunctions[i].GetExpression(); 
                } 

                Console.WriteLine(line);
            }
        }

        public static void SolveFunction(string input)
        {
            if (userFunctions.Count == 0)
            {
                Console.WriteLine("No added functions to solve. Add functions and try again.");
                return;
            }

            string[] splitName = Utility.Split(input, '(');
            string name = splitName[0];

            string valueString = Utility.TrimEnd(splitName[1], ')');
            string[] values = Utility.Split(valueString, ',');
            var boolValues = Utility.CheckBoolInput(values);

            if(boolValues == null)
            {
                Console.WriteLine("Invalid bool input. Try Again.");
                return;
            }

            var chosenFunction = ChooseFunction(name);

            if (chosenFunction.ContainsResult(Utility.Concat(boolValues)))
            {
                Console.WriteLine("Result: " + chosenFunction.GetResult(Utility.Concat(boolValues)));
                Console.WriteLine();
                return;
            }

            var tokens = Tokenizer.Tokenize(chosenFunction.GetExpression());
            var postfixTokens = Parser.ConvertToPostfix(tokens);

            Node root = Tree.CreateTree(postfixTokens, boolValues);
            var result = Tree.Evaluate(root);
            chosenFunction.AddResult(Utility.Concat(boolValues), result);

            Console.WriteLine("Result: " + result);
            Console.WriteLine();
        }

        public static LogicFunction? ChooseFunction(string name) 
        {
            for (int i = 0; i < userFunctions.Count; i++)
            {
                if (userFunctions[i].GetName() == name)
                {
                    return userFunctions[i];
                }
            }

            return null;
        }

        public static void CreateTruthTable(string input)
        {
            string[] splitName = Utility.Split(input, '(');
            string name = splitName[0];

            var chosenFunction = ChooseFunction(name);
            if(chosenFunction == null)
            {
                Console.WriteLine("This function doesn't exist.");
                return;
            }

            if (chosenFunction.GetTruthTable() != null)
            {
                PrintTruthTable(chosenFunction);
                return;
            }

            string[,] combination = new string[chosenFunction.GetOperands().Count + 1, Utility.IntPower(2, chosenFunction.GetOperands().Count)];
            string state = "True";

            int variationCount = Utility.IntPower(2, chosenFunction.GetOperands().Count) / 2;
            int repeatCount = 0;

            for (int col = 0; col < combination.GetLength(0) - 1; col++)
            {
                for (int row = 0; row < combination.GetLength(1); row++)
                {
                    if (repeatCount == variationCount)
                    {
                        switch (state)
                        {
                            case "True": state = "False"; break;
                            case "False": state = "True"; break;
                        }

                        repeatCount = 0;
                    }
                    combination[col, row] = state;
                    repeatCount++;
                }
                variationCount /= 2;
                repeatCount = 0;
                state = "True";
            }

            string boolValues = "";
            for (int row = 0; row < combination.GetLength(1); row++)
            {
                for (int col = 0; col < combination.GetLength(0) - 1; col++)
                {
                    boolValues += combination[col, row] + " ";
                }
                var tokens = Tokenizer.Tokenize(chosenFunction.GetExpression());
                var postfixTokens = Parser.ConvertToPostfix(tokens);

                Node root = Tree.CreateTree(postfixTokens, Utility.Split(boolValues, ' '));
                combination[combination.GetLength(0) - 1, row] = Tree.Evaluate(root).ToString();
                boolValues = "";
            }

            chosenFunction.SetTruthTable(combination);
            PrintTruthTable(chosenFunction);
        }

        public static void PrintTruthTable(LogicFunction logicFunction)
        {
            for (int i = 0; i < logicFunction.GetOperands().Count; i++)
            {
                Console.Write(logicFunction.GetOperands()[i] + "  " + "|" + "  ");
            }
            Console.Write("Result" + " (" + logicFunction.GetExpression() + ")");
            Console.WriteLine();

            for (int row = 0; row < logicFunction.GetTruthTable().GetLength(1); row++)
            {
                for (int col = 0; col < logicFunction.GetTruthTable().GetLength(0); col++)
                {
                    Console.Write(logicFunction.GetTruthTable()[col, row] + "  ");
                }
                Console.WriteLine();
            }
        }

        public static void FindFunction(List<string> input)
        {
            var splitLine = Utility.Split(input[0], ' ', 2);
            input[0] = splitLine[1];
            string parameter = splitLine[1];

            LogicFunction searchedFunction;

            if (Path.HasExtension(parameter))
            {
                if (!File.Exists(parameter))
                {
                    Console.WriteLine("File doesn't exist.");
                    return;
                }

                List<string> fileContent = new();

                using (var reader = new StreamReader(parameter))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        fileContent.Add(line);
                    }
                }

                searchedFunction = SearchForFunction(fileContent, ',');
            }
            else
            {
                searchedFunction = SearchForFunction(input, ' ');
            }

            if(searchedFunction == null)
            {
                Console.WriteLine("No functions with this Truth Table were found.");
                return;
            }

            Console.WriteLine("Found Function: " + searchedFunction.GetExpression());
        }

        public static LogicFunction? SearchForFunction(List<string> input, char inputSeparator)
        {
            string[,] inputTableValues = new string[Utility.SplitSize(input[0], inputSeparator), input.Count];
            int rowCounter = 0;

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == "" || input[i] == " ")
                {
                    continue;
                }
                string[] splitLine = Utility.Split(input[i], inputSeparator);

                for (int col = 0; col < inputTableValues.GetLength(0); col++)
                {
                    inputTableValues[col, rowCounter] = splitLine[col];
                }
                rowCounter++;
            }

            bool tableMatch = true;

            for (int i = 0; i < userFunctions.Count; i++)
            {
                if (userFunctions[i].GetTruthTable() == null)
                {
                    continue;
                }
                else if (userFunctions[i].GetTruthTable().GetLength(1) != inputTableValues.GetLength(1)
                    || userFunctions[i].GetTruthTable().GetLength(0) != inputTableValues.GetLength(0))
                {
                    continue;
                }

                for (int row = 0; row < userFunctions[i].GetTruthTable().GetLength(1); row++)
                {
                    if (!tableMatch)
                    {
                        break;
                    }

                    for (int col = 0; col < userFunctions[i].GetTruthTable().GetLength(0); col++)
                    {
                        if (userFunctions[i].GetTruthTable()[col, row] != inputTableValues[col, row])
                        {
                            tableMatch = false;
                        }
                    }
                }

                if (tableMatch)
                {
                    return userFunctions[i];
                }
            }

            return null;
        }
    }
}
