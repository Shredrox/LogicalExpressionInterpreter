﻿using LogicalExpressionInterpreter.BinaryTree;
using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public static class LogicController
    {
        private static DynamicArray<string> userFunctions = new();

        public static void Run()
        {
            if (File.Exists("../../UserFunctions.txt"))
            {
                userFunctions = DataControl.LoadFromFile("../../UserFunctions.txt");
            }

            //string input1 = "(((( a || b ) && c ) || ( d && p ) ) && e ) && ( f || g )";
            //string e = "(a && b) || (c && ((d || e) && f))";
            //string e2 = "a && ( !( b || c ) || d ) && e";
            //string e3 = "aaa";
            //userFunctions.Add(input1);
            //userFunctions.Add(e);
            //userFunctions.Add(e2);

            while (true)
            {
                Console.WriteLine("Choose command from menu: ");
                Console.WriteLine("1. Add Function");
                Console.WriteLine("2. Print Added Functions");
                Console.WriteLine("3. Solve Function");
                Console.WriteLine("4. Create Truth Table");
                Console.WriteLine("5. Load Truth Table");
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
                    case 2: PrintFunctions(); break;
                    case 3: SolveFunction(); break;
                    case 7: return;
                }

                Console.WriteLine();
            }
        }

        public static void AddFunction()
        {
            Console.WriteLine("Enter new bool expression/function: ");
            userFunctions.Add(Console.ReadLine());
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
                Console.WriteLine(i + 1 + ": " + userFunctions[i]);
            }
        }

        public static void SolveFunction()
        {
            Console.WriteLine("Choose function to solve: ");
            PrintFunctions();

            int choice = int.Parse(Console.ReadLine()) - 1;

            Console.WriteLine("Enter bool values: ");
            string values = Console.ReadLine();
            string[] boolInput = Utility.Split(values, ' ');

            var tokens = Tokenizer.Tokenize(userFunctions[choice]);
            var postfixTokens = Parser.ConvertToPostfix(tokens);

            Node root = Tree.CreateTree(postfixTokens, boolInput);

            Console.WriteLine("Result: " + Tree.Evaluate(root));
            Console.WriteLine();
        }
    }
}
