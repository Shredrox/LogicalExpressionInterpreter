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

        public static List<LogicFunction> GetFunctionsWithOperandCount(int operandCount)
        {
            List<LogicFunction> matchingFunctions = new();
            for (int i = 0; i < userFunctions.Count; i++)
            {
                if (userFunctions[i].GetOperands().Count <= operandCount)
                {
                    matchingFunctions.Add(userFunctions[i]);
                }
            }

            return matchingFunctions;
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
                    case "DEFINE": 
                        {
                            AddFunction(inputSplit[1], out string errorMsg);

                            if (errorMsg != "")
                            {
                                Console.WriteLine(errorMsg);
                            }
                            else
                            {
                                
                            }
                            
                            break;
                        }
                    case "REMOVE": RemoveFunction(inputSplit[1]); SaveFunctions(); break;
                    case "PRINTALL": PrintFunctions(); break;
                    case "SOLVE": 
                        {
                            string result = SolveFunction(inputSplit[1], out string errorMsg);
                            if (errorMsg != "")
                            {
                                Console.WriteLine(errorMsg);
                            }
                            else
                            {
                                Console.WriteLine(result);
                            }

                            break;
                        } 
                    case "ALL": 
                        {
                            CreateTruthTable(inputSplit[1], out LogicFunction? functionWithCreatedTable); 

                            if(functionWithCreatedTable == null)
                            {
                                Console.WriteLine("This function doesn't exist.");
                            }
                            else
                            {
                                PrintTruthTable(functionWithCreatedTable);
                            }

                            break;
                        } 
                    case "FIND": 
                        {
                            string result = FindFunction(inputLines, out string errorMsg);
                            if(errorMsg != "")
                            {
                                Console.WriteLine(errorMsg);
                            }
                            else
                            {
                                Console.WriteLine(result);
                            }

                            break;
                        } 
                    case "EXIT": SaveFunctions(); return;
                    default: Console.WriteLine("Invalid Command."); break;
                }

                Console.WriteLine();
            }
        }

        public static void AddFunction(string input, out string errorMsg)
        {
            errorMsg = "";
            string[] inputSplit = Utility.Split(input, ':');
            string[] splitName = Utility.Split(inputSplit[0], '(');
            string name = splitName[0];

            var tokens = Tokenizer.Tokenize(inputSplit[1]);

            if (FunctionExists(name))
            {
                errorMsg = "A function with this name already exists.";
                return;
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
                    var nestedFunction = ChooseFunction(nestedFunctionName);

                    if (nestedFunction != null)
                    {
                        nestedFunctions.Add(nestedFunction);
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
                errorMsg = "Invalid number of operands.";
                return;
            }
            else if (hasNestedFunction && nestedFunctions.Count == 0)
            {
                errorMsg = "Nested Function: " + nestedFunctionName + " doesn't exist.";
                return;
            }

            var newFunction = new LogicFunction(name, expression, inputSplit[0], tokens);
            newFunction.SetNestedFunctions(nestedFunctions);
            userFunctions.Add(newFunction);
        }

        public static string SolveFunction(string input, out string errorMsg)
        {
            errorMsg = "";
            if (userFunctions.Count == 0)
            {
                errorMsg = "No added functions to solve. Add functions and try again.";
                return "";
            }

            string[] splitName = Utility.Split(input, '(');
            string name = splitName[0];

            string valueString = Utility.TrimEnd(splitName[1], ')');
            string[] values = Utility.Split(valueString, ',');
            var boolValues = Utility.CheckBoolInput(values);

            if(boolValues == null)
            {
                errorMsg = "Invalid bool input. Try Again.";
                return "";
            }

            var chosenFunction = ChooseFunction(name);
            if(chosenFunction == null)
            {
                errorMsg = "Function doesn't exist";
                return "";
            }

            if (chosenFunction.ContainsResult(Utility.Concat(boolValues)))
            {
                return "Result: " + chosenFunction.GetResult(Utility.Concat(boolValues));
            }

            var tokens = Tokenizer.Tokenize(chosenFunction.GetExpression());
            var postfixTokens = Parser.ConvertToPostfix(tokens);

            Node root = Tree.CreateTree(postfixTokens, boolValues);
            var result = Tree.Evaluate(root);
            chosenFunction.AddResult(Utility.Concat(boolValues), result);

            return "Result: " + result;
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

        public static List<string>? GetFunctionsInfo()
        {
            if (userFunctions.Count == 0)
            {
                return null;
            }

            List<string> functionsInfo = new();
            functionsInfo.Add("Current Functions: ");
            functionsInfo.Add("\n");
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
                functionsInfo.Add("\n");
            }

            return functionsInfo;
        }

        public static void PrintFunctions()
        {
            var functionsInfo = GetFunctionsInfo();
            if (functionsInfo == null)
            {
                Console.WriteLine("No added functions.");
                return;
            }

            for (int i = 0; i < functionsInfo.Count; i++)
            {
                Console.Write(functionsInfo[i]);
            }
        }

        public static void CreateTruthTable(string input, out LogicFunction? functionWithCreatedTable)
        {
            functionWithCreatedTable = null;
            string[] splitName = Utility.Split(input, '(');
            string name = splitName[0];

            var chosenFunction = ChooseFunction(name);
            if(chosenFunction == null)
            {
                return;
            }

            if (chosenFunction.GetTruthTable() != null)
            {
                functionWithCreatedTable = chosenFunction;
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
            functionWithCreatedTable = chosenFunction;     
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

        public static string FindFunction(List<string> input, out string errorMsg)
        {
            errorMsg = "";
            var splitLine = Utility.Split(input[0], ' ', 2);
            input[0] = splitLine[1];
            string parameter = splitLine[1];

            string searchedFunction;

            if (Path.HasExtension(parameter))
            {
                if (!File.Exists(parameter))
                {
                    errorMsg = "File doesn't exist.";
                    return "";
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
                searchedFunction = SearchForFunction(input, ',');
            }

            return "Result: " + searchedFunction;
        }

        public static string SearchForFunction(List<string> input, char inputSeparator)
        {
            string[,] inputTableValues = new string[input.Count, Utility.SplitSize(input[0], inputSeparator)];
            int rowCounter = 0;

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == "" || input[i] == " ")
                {
                    continue;
                }
                string[] splitLine = Utility.Split(input[i], inputSeparator);

                for (int col = 0; col < inputTableValues.GetLength(1); col++)
                {
                    inputTableValues[rowCounter, col] = splitLine[col];
                }
                rowCounter++;
            }

            string[,] table =
            {
                { "false", "false", "false", "false"},
                { "false", "false", "true", "false"},
                { "false", "true", "false", "false"},
                { "false", "true", "true", "false"},
                { "true", "false", "false", "true"},
                { "true", "false", "true", "false"},
                { "true", "true", "false", "false"},
                { "true", "true", "true", "false"},
            };

            string[,] table2 =
            {
                { "true", "true", "true", "true", "false"},
                { "true", "true", "true", "false", "false"},
                { "true", "true", "false", "true", "false"},
                { "true", "true", "false", "false", "false"},
                { "true", "false", "true", "true", "false"},
                { "true", "false", "true", "false", "false"},
                { "true", "false", "false", "true", "false"},
                {"true","false","false","false","false" },
                {"false", "true","true","true","false" },
                {"false", "true","true","false","false" },
                {"false","true","false","true","false" },
                {"false","true","false","false","true" },
                {"false","false","true","true","false" },
                {"false", "false","true","false","false" },
                {"false","false","false","true","false" },
                {"false","false","false","false","false" }
            };

            var foundFunction = Evolution.ConstructBooleanExpression(table);
            if(foundFunction == "")
            {
                return "No function was found.";
            }
            //var result = Parser.ConvertToInfix(foundFunction);

            var functions = GetFunctionsWithOperandCount(table.GetLength(1)-1);
            List<LogicFunction> test = new();
            test.Add(new LogicFunction("test1", "a & b && c", "test1(a,b,c)"));
            test.Add(new LogicFunction("test2", "a || b", "test2(a,b)"));
            test.Add(new LogicFunction("test3", "a & b & c", "test3(a,b,c)"));

            var finalResult = CombineResult(foundFunction, functions);

            return finalResult;
        }

        public static string CombineResult(string evolutionExpression, List<LogicFunction> functions)
        {
            var tokens = Tokenizer.Tokenize(evolutionExpression);

            for (int i = 0; i < functions.Count; i++)
            {
                var functionTokens = Tokenizer.Tokenize(functions[i].GetExpression());
                var functionIndex = ContainedTokensStartIndex(tokens, functionTokens);

                if (functionIndex != -1)
                {
                    string functionName = functions[i].GetCombinedName();
                    return evolutionExpression + "  <--or-->  " + FormatResult(tokens, functionTokens, functionName,functionIndex);
                }
            }

            return evolutionExpression;
        }

        public static string FormatResult(List<Token> tokens, List<Token> subTokens, string functionName, int index)
        {
            string result = "";

            for (int i = 0; i < tokens.Count; i++)
            {
                if (i == index)
                {
                    result += functionName + " ";
                    i += subTokens.Count - 1;
                    continue;
                }
                result += tokens[i].Value + " ";
            }

            return result;
        }

        public static int ContainedTokensStartIndex(List<Token> tokens, List<Token> subTokens)
        {
            int counter = 0;
            int subTokensIndex = 0;
            int index = 0;
            bool atSubStartPoint = false;

            for (int i = 0; i <= tokens.Count; i++)
            {
                if (counter == subTokens.Count)
                {
                    return index;
                }
                else if (i == tokens.Count)
                {
                    return -1;
                }

                if (tokens[i].Type != subTokens[subTokensIndex].Type)
                {
                    if (counter > 0)
                    {
                        counter--;
                    }
                    index = -1;
                    subTokensIndex = 0;
                }
                else if (tokens[i].Type == subTokens[subTokensIndex].Type)
                {
                    atSubStartPoint = true;
                    subTokensIndex++;
                    counter++;
                }

                if (atSubStartPoint && counter == 1)
                {
                    index = i;
                    atSubStartPoint = false;
                }
            }

            return -1;
        }
    }
}
