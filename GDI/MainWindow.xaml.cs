﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LogicalExpressionInterpreter.BinaryTree;
using LogicalExpressionInterpreter.LogicControl;
using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace GDI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LogicController.LoadFunctions();
        }

        public void ProcessInput(string input)
        {
            TextDisplay.Inlines.Clear();

            var inputSplit = Utility.Split(input, ' ', 2);

            if (Utility.StringIsNullOrEmpty(inputSplit[1])
                    && Utility.ToUpper(inputSplit[0]) != "PRINTALL"
                    && Utility.ToUpper(inputSplit[0]) != "EXIT"
                    && Utility.ToUpper(inputSplit[0]) != "HELP")
            {
                MessageBox.Show("Invalid Command.");
                return;
            }

            switch (Utility.ToUpper(inputSplit[0]))
            {
                case "DEFINE": AddFunction(inputSplit[1]); LogicController.SaveFunctions(); break;
                case "REMOVE": LogicController.RemoveFunction(inputSplit[1]); LogicController.SaveFunctions(); break;
                case "PRINTALL": PrintFunctions(); break;
                case "SOLVE": SolveFunction(inputSplit[1]); break;
                case "ALL": CreateTruthTable(inputSplit[1]); break;
                case "DISPLAY": DisplayTree(inputSplit[1]); break;  
                case "FIND": break; //FindFunction(inputSplit[1]);
                case "HELP": PrintCommands(); break;
                case "EXIT": LogicController.SaveFunctions(); this.Close(); return;
                default: MessageBox.Show("Invalid Command."); break;
            }
        }

        public void PrintCommands()
        {
            TextDisplay.Inlines.Add("DEFINE - defines a function\n");
            TextDisplay.Inlines.Add("REMOVE - removes a function\n");
            TextDisplay.Inlines.Add("SOLVE - solves a function with given bool parameters\n");
            TextDisplay.Inlines.Add("ALL - creates a truth table for a function\n");
            TextDisplay.Inlines.Add("FIND - finds a function from a given truth table\n");
            TextDisplay.Inlines.Add("PRINTALL - prints all functions\n");
            TextDisplay.Inlines.Add("DISPLAY - displays function binary tree\n");
            TextDisplay.Inlines.Add("EXIT - closes the program\n");
        }

        public void AddFunction(string input)
        {
            string[] inputSplit = Utility.Split(input, ':');
            string[] splitName = Utility.Split(inputSplit[0], '(');
            string name = splitName[0];

            var tokens = Tokenizer.Tokenize(inputSplit[1]);

            if(LogicController.FunctionExists(name))
            {
                MessageBox.Show("A function with this name already exists.");
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
                    var nestedFunction = LogicController.ChooseFunction(nestedFunctionName);

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
                MessageBox.Show("Invalid number of operands.");
                return;
            }
            else if (hasNestedFunction && nestedFunctions.Count == 0)
            {
                MessageBox.Show("Nested Function: " + nestedFunctionName + " doesn't exist.");
                return;
            }

            var newFunction = new LogicFunction(name, expression, inputSplit[0], tokens);
            newFunction.SetNestedFunctions(nestedFunctions);
            LogicController.AddUserFunction(newFunction);
        }

        public void PrintFunctions()
        {
            if(LogicController.GetFunctionCount() == 0)
            {
                MessageBox.Show("No added functions.");
                return;
            }

            TextDisplay.Inlines.Add("Current Functions: ");
            TextDisplay.Inlines.Add("\n");

            var info = LogicController.GetFunctionsInfo();

            for (int i = 0; i < info.Count; i++)
            {
                TextDisplay.Inlines.Add(info[i]);
                TextDisplay.Inlines.Add("\n");
            }
        }

        public void SolveFunction(string input)
        {
            if (LogicController.GetFunctionCount() == 0)
            {
                MessageBox.Show("No added functions to solve. Add functions and try again.");
                return;
            }

            string[] splitName = Utility.Split(input, '(');
            string name = splitName[0];

            string valueString = Utility.TrimEnd(splitName[1], ')');
            string[] values = Utility.Split(valueString, ',');
            var boolValues = Utility.CheckBoolInput(values);

            if (boolValues == null)
            {
                MessageBox.Show("Invalid bool input. Try Again.");
                return;
            }

            var chosenFunction = LogicController.ChooseFunction(name);

            if(chosenFunction == null)
            {
                MessageBox.Show("Function doesn't exist.");
                return;
            }

            if (chosenFunction.ContainsResult(Utility.Concat(boolValues)))
            {
                TextDisplay.Inlines.Add("Result: " + chosenFunction.GetResult(Utility.Concat(boolValues)));
                return;
            }

            var tokens = Tokenizer.Tokenize(chosenFunction.GetExpression());
            var postfixTokens = Parser.ConvertToPostfix(tokens);

            Node root = Tree.CreateTree(postfixTokens, boolValues);
            var result = Tree.Evaluate(root);
            chosenFunction.AddResult(Utility.Concat(boolValues), result);

            TextDisplay.Inlines.Add("Result: " + result);
        }

        public void CreateTruthTable(string input)
        {
            string[] splitName = Utility.Split(input, '(');
            string name = splitName[0];

            var chosenFunction = LogicController.ChooseFunction(name);
            if (chosenFunction == null)
            {
                MessageBox.Show("This function doesn't exist.");
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

        public void PrintTruthTable(LogicFunction logicFunction)
        {
            if(logicFunction.GetTruthTable() == null)
            {
                MessageBox.Show("Truth table is null.");
                return;
            }

            string line = "";
            for (int i = 0; i < logicFunction.GetOperands().Count; i++)
            {
                line += logicFunction.GetOperands()[i] + "\t" + "|" + " ";
            }
            line += "Result" + " (" + logicFunction.GetExpression() + ")";
            TextDisplay.Inlines.Add(line);
            TextDisplay.Inlines.Add("\n");

            line = "";

            for (int row = 0; row < logicFunction.GetTruthTable().GetLength(1); row++)
            {
                for (int col = 0; col < logicFunction.GetTruthTable().GetLength(0); col++)
                {
                    line += logicFunction.GetTruthTable()[col, row] + "\t| ";
                }
                TextDisplay.Inlines.Add(line);
                TextDisplay.Inlines.Add("\n");
                line = "";
            }
        }

        public void FindFunction(List<string> input)
        {
            var splitLine = Utility.Split(input[0], ' ', 2);
            input[0] = splitLine[1];
            string parameter = splitLine[1];

            LogicFunction? searchedFunction;

            if (System.IO.Path.HasExtension(parameter))
            {
                if (!File.Exists(parameter))
                {
                    MessageBox.Show("File doesn't exist.");
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

                searchedFunction = LogicController.SearchForFunction(fileContent, ',');
            }
            else
            {
                searchedFunction = LogicController.SearchForFunction(input, ' ');
            }

            if(searchedFunction == null)
            {
                MessageBox.Show("No functions with this Truth Table were found.");
                return;
            }

            TextDisplay.Inlines.Add("Result: " + searchedFunction.GetExpression());
        }

        private double heightDivide;
        private double widthDivide;
        private void DisplayTree(string functionName)
        {
            TreeCanvas.Children.Clear();

            var chosenFunction = LogicController.ChooseFunction(functionName);
            if (chosenFunction == null)
            {
                MessageBox.Show("This function doesn't exist.");
                return;
            }

            var tokens = Tokenizer.Tokenize(chosenFunction.GetExpression());
            var postfixTokens = Parser.ConvertToPostfix(tokens);

            Node root = Tree.CreateTree(postfixTokens);

            var depth = Tree.TreeDepth(root);
            heightDivide = TreeCanvas.ActualHeight / depth / 2;
            widthDivide = 100;

            AddNodeToCanvas(root, 0, heightDivide, 1, 0);
        }

        private void AddNodeToCanvas(Node? root, double xOffset, double yOffset, double xDivider, int nodePos)
        {
            if (root.GetLeft() != null)
            {
                AddNodeToCanvas(root.GetLeft(), xOffset + widthDivide / (xDivider * 1.3d), yOffset + heightDivide * 2, xDivider * 1.3d, 2);
            }

            var ellipse = new Ellipse
            {
                Height = 30,
                Width = 30
            };
            ellipse.Margin = new Thickness(-ellipse.Height / 2);
            ellipse.Fill = Brushes.Yellow;

            Grid container = new();
            container.SetValue(Canvas.LeftProperty, TreeCanvas.Width / 2 + (xOffset));
            container.SetValue(Canvas.TopProperty, yOffset);
            container.SetValue(Canvas.ZIndexProperty, 2);
            container.Children.Add(ellipse);
            container.Children.Add(new TextBlock() { Text = root.GetValue() });

            Line line = new()
            {
                X1 = Math.Round(Convert.ToDouble(container.GetValue(LeftProperty))),
                Y1 = Math.Round(Convert.ToDouble(container.GetValue(TopProperty)))
            };
            line.Y2 = line.Y1 - heightDivide * 2;

            switch (nodePos)
            {
                case 0: line.X2 = line.X1; line.Y2 = line.Y1; break;
                case 1: line.X2 = Math.Round(line.X1 + (widthDivide / xDivider) + 5); break;
                case 2: line.X2 = Math.Round(line.X1 - (widthDivide / xDivider) + 5); break;
            }

            line.Stroke = Brushes.Yellow;

            TreeCanvas.Children.Add(container);
            TreeCanvas.Children.Add(line);

            if (root.GetRight() != null)
            {
                AddNodeToCanvas(root.GetRight(), xOffset - widthDivide / (xDivider * 1.3d), yOffset + heightDivide * 2, xDivider * 1.3d, 1);
            }
        }

        //Window functions
        private void CommandInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string input = Utility.TrimEnd(CommandInput.Text, '\n');
                string[] split = Utility.Split(input, '\r');

                if (split[split.Length-1] == null)
                {
                    return;
                }

                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = Utility.TrimStart(split[i], '\n');
                }

                ProcessInput(split[split.Length - 1]);
            }
        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
