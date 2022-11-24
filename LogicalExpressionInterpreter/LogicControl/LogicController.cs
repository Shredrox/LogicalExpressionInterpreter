using LogicalExpressionInterpreter.BinaryTree;
using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public static class LogicController
    {
        private static List<LogicFunction> userFunctions = new();

        public static void Run()
        {
            if (File.Exists("../../UserFunctions.txt"))
            {
                userFunctions = DataControl.LoadFromFile("../../UserFunctions.txt");
            }

            //string input1 = "(((( a || b ) && c ) || ( d && p ) ) && e ) && ( f || g )";
            //string e = "(a && b) || (c && ((d || e) && f))";
            //string e2 = "a && ( !( b || c ) || d ) && e";
            //string e3 = "a || ((b || c) && d)";
            //string e3 = "a && b";

            while (true)
            {
                Console.WriteLine("Enter command: ");

                string input = Console.ReadLine();
                var inputSplit = Utility.Split(input, ' ', 2);

                switch (inputSplit[0].ToUpper())
                {
                    case "DEFINE": AddFunction(inputSplit[1]); break;
                    case "SOLVE": break;
                    case "ALL": break;
                    case "FIND": break;
                    case "EXIT": DataControl.SaveToFile(userFunctions, "../../UserFunctions.txt"); return;
                    default: Console.WriteLine("Invalid Command."); break;
                }

                break;
            }

            while (true)
            {
                Console.WriteLine("Choose command from menu: ");
                Console.WriteLine("1. Add Function");
                Console.WriteLine("2. Remove Function");
                Console.WriteLine("3. Print Added Functions");
                Console.WriteLine("4. Solve Function");
                Console.WriteLine("5. Create Truth Table From Function");
                Console.WriteLine("6. Find Logic Function In Table");
                Console.WriteLine("7. Exit");

                string input = Console.ReadLine();
                if (!int.TryParse(input, out _))
                {
                    Console.WriteLine("Invalid command. Try again.");
                    continue;
                }
                int command = int.Parse(input);

                switch (command)
                {
                    case 1: AddFunction(); DataControl.SaveToFile(userFunctions, "../../UserFunctions.txt"); break;
                    case 2: RemoveFunction(); DataControl.SaveToFile(userFunctions, "../../UserFunctions.txt"); break;
                    case 3: PrintFunctions(); break;
                    case 4: SolveFunction(); break;
                    case 5: CreateTable(); break;
                    case 6: FindFunctionFromTruthTable(); break;
                    case 7: DataControl.SaveToFile(userFunctions, "../../UserFunctions.txt"); return;
                }

                Console.WriteLine();
            }
        }

        public static void AddFunction(string input)
        {
            string[] inputSplit = Utility.Split(input, ':');
            string[] splitName = Utility.Split(inputSplit[0], '(');
            string name = splitName[0];
            string operands = Utility.TrimEnd(splitName[1], ')');
            string expression = inputSplit[1];

            userFunctions.Add(new LogicFunction(name, expression, inputSplit[0]));
        }

        public static void AddFunction()
        {
            Console.WriteLine("Choose option:");
            Console.WriteLine("1. Add new function");
            Console.WriteLine("2. Include existing function in new function.");

            bool valid = false;
            int input = 0;

            while (!valid)
            {
                valid = int.TryParse(Console.ReadLine(), out input);

                if (valid && (input == 1 || input == 2))
                {
                    break;
                }

                Console.WriteLine("Invalid input! Try again.");
                valid = false;
            }

            switch (input)
            {
                case 1:
                    {
                        Console.WriteLine("Enter function name: ");
                        string name = Console.ReadLine();
                        Console.WriteLine("Enter new bool expression/function: ");
                        string expression = Console.ReadLine(); 
                        userFunctions.Add(new LogicFunction(name, expression));
                        break;
                    }
                case 2:
                    {
                        var chosenFunction = ChooseFunction();

                        Console.WriteLine("Enter function name: ");
                        string name = Console.ReadLine();
                        Console.WriteLine("Enter new bool expression/function: ");
                        Console.Write(chosenFunction.GetExpression() + " ");
                        string expression = Console.ReadLine();
                        
                        var newFunction = new LogicFunction(name, expression);
                        newFunction.AddNestedFunction(chosenFunction);

                        userFunctions.Add(newFunction);
                        break;
                    }
            }
        }

        private static void RemoveFunction()
        {
            Console.WriteLine("Choose function to delete: ");
            PrintFunctions();

            int choice = int.Parse(Console.ReadLine()) - 1;
            userFunctions.RemoveAt(choice);
        }

        public static void PrintFunctions()
        {
            if (userFunctions.Count == 0)
            {
                Console.WriteLine("No added functions.");
                return;
            }

            Console.WriteLine("Current Functions: ");

            string line = "";

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

        public static LogicFunction? ChooseFunction()
        {
            if (userFunctions.Count == 0)
            {
                return null;
            }

            Console.WriteLine("Choose function: ");
            PrintFunctions();

            bool valid = false;

            while (!valid)
            {
                valid = int.TryParse(Console.ReadLine(), out int input);

                if (valid && input >= 1 && input <= userFunctions.Count)
                {
                    return userFunctions[input - 1];
                }

                Console.WriteLine("Invalid input! Try again.");
                PrintFunctions();
                valid = false;
            }

            return null;
        }

        public static void SolveFunction()
        {
            var chosenFunction = ChooseFunction();
            if(chosenFunction == null)
            {
                Console.WriteLine("No added functions to solve. Add functions and try again.");
                return;
            }

            Console.WriteLine("Enter bool values: ");
            string[] boolInput;
            while (true)
            {
                boolInput = CheckBoolInput(Console.ReadLine());
                if (boolInput == null)
                {
                    Console.WriteLine("Invalid Input. Try Again.");
                    continue;
                }

                break;
            }

            var tokens = Tokenizer.Tokenize(chosenFunction.GetExpression());
            var postfixTokens = Parser.ConvertToPostfix(tokens);

            Node root = Tree.CreateTree(postfixTokens, boolInput);

            Console.WriteLine("Result: " + Tree.Evaluate(root));
            Console.WriteLine();
        }

        public static string[]? CheckBoolInput(string input)
        {
            string[] boolValues = Utility.Split(input, ' ');
            for (int i = 0; i < boolValues.Length; i++)
            {
                if (!bool.TryParse(boolValues[i], out _))
                {
                    return null;
                }
            }

            return boolValues;
        }

        public static void CreateTable()
        {
            var chosenFunction = ChooseFunction();

            if (chosenFunction.GetTruthTable() != null)
            {
                PrintTruthTable(chosenFunction);
                return;
            }

            string[,] combination = new string[chosenFunction.GetOperands().Count + 1, Utility.IntPower(2, chosenFunction.GetOperands().Count)];
            string state = "True";

            int variationCount = Utility.IntPower(2, chosenFunction.GetOperands().Count) / 2;
            int repeatCount = 0;

            for (int col = 0; col < combination.GetLength(0)-1; col++)
            {
                for (int row = 0; row < combination.GetLength(1); row++)
                {
                    if(repeatCount == variationCount)
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

        public static void FindFunctionFromTruthTable()
        {
            Console.WriteLine("Choose option:");
            Console.WriteLine("1. From File (.csv)");
            Console.WriteLine("2. From Input");

            bool valid = false;
            int input = 0;

            while (!valid)
            {
                valid = int.TryParse(Console.ReadLine(), out input);

                if (valid && (input == 1 || input == 2))
                {
                    break;
                }

                Console.WriteLine("Invalid input! Try again.");
                valid = false;
            }

            switch (input)
            {
                case 1: GetTruthTableFile(); break;
                case 2: GetTruthTableInput(); break;
            }
        }

        public static void GetTruthTableFile()
        {
            Console.WriteLine("Enter file path: ");
            string path = Console.ReadLine();
            if(!File.Exists(path))
            {
                Console.WriteLine("File doesn't exist.");
                return;
            }

            List<string> input = new();

            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    input.Add(line);
                }
            }

            SearchForFunction(input, ',');
        }

        public static void GetTruthTableInput()
        {
            Console.WriteLine("Enter Truth Table: ");
            string line = "1";
            List<string> input = new();
            while (line != "" && line != " ")
            {
                line = Console.ReadLine();
                if (line == "" || line == " ")
                {
                    continue;
                }
                input.Add(line);
            }

            SearchForFunction(input, ' ');
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
                    Console.WriteLine("Found Function: " + userFunctions[i].GetExpression());
                    return userFunctions[i];
                }
            }

            Console.WriteLine("No functions with this Truth Table were found.");

            return null;
        }
    }
}
