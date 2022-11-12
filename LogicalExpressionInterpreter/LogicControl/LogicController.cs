﻿using LogicalExpressionInterpreter.BinaryTree;
using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public static class LogicController
    {
        private static DynamicArray<LogicFunction> userFunctions = new();

        public static void Run()
        {
            if (File.Exists("../../UserFunctions.txt"))
            {
                userFunctions = DataControl.LoadFromFile("../../UserFunctions.txt");
            }

            //string input1 = "(((( a || b ) && c ) || ( d && p ) ) && e ) && ( f || g )";
            string e = "(a && b) || (c && ((d || e) && f))";
            //string e2 = "a && ( !( b || c ) || d ) && e";
            //string e3 = "aaa";
            //userFunctions.Add(input1);
            //userFunctions.Add(e);
            //userFunctions.Add(e2);

            var test = new LogicFunction(e);

            while (true)
            {
                Console.WriteLine("Choose command from menu: ");
                Console.WriteLine("1. Add Function");
                Console.WriteLine("2. Remove Function");
                Console.WriteLine("3. Print Added Functions");
                Console.WriteLine("4. Solve Function");
                Console.WriteLine("5. Create Truth Table From Function");
                Console.WriteLine("6. Load Truth Table");
                Console.WriteLine("7. Find Logic Function In Table");
                Console.WriteLine("8. Exit");

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
                    case 8: return;
                }

                Console.WriteLine();
            }
        }

        public static void AddFunction()
        {
            Console.WriteLine("Enter new bool expression/function: ");
            userFunctions.Add(new LogicFunction(Console.ReadLine()));
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

            for (int i = 0; i < userFunctions.Count; i++)
            {
                Console.WriteLine(i + 1 + ": " + userFunctions[i].GetExpression());
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

            string input = Console.ReadLine();
            bool validInput = false;
            while (!validInput)
            {
                if(!int.TryParse(input, out _))
                {
                    Console.WriteLine("Invalid input! Try again.");
                    PrintFunctions();
                    input = Console.ReadLine();
                }
                else if (int.Parse(input) < 1 || int.Parse(input) > userFunctions.Count)
                {
                    Console.WriteLine("Invalid input! Try again.");
                    PrintFunctions();
                    input = Console.ReadLine();
                }
                else
                {
                    validInput = true;
                }
            }

            return userFunctions[int.Parse(input) - 1];
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
            string values = Console.ReadLine();
            string[] boolInput = Utility.Split(values, ' ');

            var tokens = Tokenizer.Tokenize(chosenFunction.GetExpression());
            var postfixTokens = Parser.ConvertToPostfix(tokens);

            Node root = Tree.CreateTree(postfixTokens, boolInput);

            Console.WriteLine("Result: " + Tree.Evaluate(root));
            Console.WriteLine();
        }

        public static void CreateTable()
        {
            var chosenFunction = ChooseFunction();
        }
    }
}
