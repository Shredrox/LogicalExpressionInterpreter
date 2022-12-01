using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using LogicalExpressionInterpreter.BinaryTree;
using LogicalExpressionInterpreter.LogicControl;
using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace GDI
{
    public partial class MainWindow : Window
    {
        private static List<LogicFunction> userFunctions = new();
        private string path = "../../../../LogicalExpressionInterpreter/bin/UserFunctions.txt";

        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists(path))
            {
                userFunctions = DataControl.LoadFromFile(path);
            }
        }

        public void ProcessInput(string input)
        {
            TextDisplay.Inlines.Clear();

            var inputSplit = Utility.Split(input, ' ', 2);

            if (Utility.StringIsNullOrEmpty(inputSplit[1])
                    && Utility.ToUpper(inputSplit[0]) != "PRINTALL"
                    && Utility.ToUpper(inputSplit[0]) != "EXIT")
            {
                MessageBox.Show("Invalid Command.");
                return;
            }

            switch (Utility.ToUpper(inputSplit[0]))
            {
                case "DEFINE": AddFunction(inputSplit[1]); DataControl.SaveToFile(userFunctions, path); break;
                case "REMOVE": RemoveFunction(inputSplit[1]); DataControl.SaveToFile(userFunctions, path); break;
                case "PRINTALL": PrintFunctions(); break;
                case "SOLVE": SolveFunction(inputSplit[1]); break;
                case "ALL": CreateTruthTable(inputSplit[1]); break;
                case "DISPLAY": DisplayTree(inputSplit[1]); break;  
                case "FIND": //FindFunction(inputSplit[1]); break;
                    break;
                case "EXIT": DataControl.SaveToFile(userFunctions, "../../UserFunctions.txt"); return;
                default: MessageBox.Show("Invalid Command."); break;
            }
        }

        public void AddFunction(string input)
        {
            string[] inputSplit = Utility.Split(input, ':');
            string[] splitName = Utility.Split(inputSplit[0], '(');
            string name = splitName[0];

            var tokens = Tokenizer.Tokenize(inputSplit[1]);

            for (int i = 0; i < userFunctions.Count; i++)
            {
                if (userFunctions[i].GetName() == name)
                {
                    MessageBox.Show("A function with this name already exists.");
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
            userFunctions.Add(newFunction);
        }

        public void RemoveFunction(string input)
        {
            for (int i = 0; i < userFunctions.Count; i++)
            {
                if (userFunctions[i].GetName() == input)
                {
                    userFunctions.RemoveAt(i);
                }
            }
        }

        public void PrintFunctions()
        {
            if (userFunctions.Count == 0)
            {
                MessageBox.Show("No added functions.");
                return;
            }

            TextDisplay.Inlines.Add("Current Functions: ");
            TextDisplay.Inlines.Add("\n");

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

                TextDisplay.Inlines.Add(line);
                TextDisplay.Inlines.Add("\n");
            }
        }

        public void SolveFunction(string input)
        {
            if (userFunctions.Count == 0)
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

            var chosenFunction = ChooseFunction(name);

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

        public void CreateTruthTable(string input)
        {
            string[] splitName = Utility.Split(input, '(');
            string name = splitName[0];

            var chosenFunction = ChooseFunction(name);
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

        public void FindFunction(List<string> input)
        {
            var splitLine = Utility.Split(input[0], ' ', 2);
            input[0] = splitLine[1];
            string parameter = splitLine[1];

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

                SearchForFunction(fileContent, ',');
            }
            else
            {
                SearchForFunction(input, ' ');
            }
        }

        public LogicFunction? SearchForFunction(List<string> input, char inputSeparator)
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

            MessageBox.Show("No functions with this Truth Table were found.");

            return null;
        }

        public static Node CreateTree(List<Token> tokens)
        {
            ObjectStack<Node> nodes = new();

            for (int i = 0; i < tokens.Count; i++)
            {
                switch (tokens[i].Type)
                {
                    case Token.TokenType.NESTED_FUNCTION:
                        {
                            var nestedFunction = ChooseFunction(Utility.Split(tokens[i].Value, '(')[0]);
                            var nestedNode = CreateTree(Parser.ConvertToPostfix(nestedFunction.GetTokens()));
                            nodes.Push(nestedNode);
                            break;
                        }
                    case Token.TokenType.LITERAL: nodes.Push(new Node(tokens[i].Value)); break;
                    case Token.TokenType.NOT:
                        {
                            Node left = nodes.Pop();
                            nodes.Push(new Node(tokens[i].Value, left));
                            break;
                        }
                    default:
                        {
                            Node left = nodes.Pop();
                            Node right = nodes.Pop();
                            nodes.Push(new Node(tokens[i].Value, left, right));
                            break;
                        }
                }
            }

            return nodes.Pop();
        }

        private void DisplayTree(string functionName)
        {
            TreeCanvas.Children.Clear();

            var chosenFunction = ChooseFunction(functionName);
            if (chosenFunction == null)
            {
                MessageBox.Show("This function doesn't exist.");
                return;
            }

            var tokens = Tokenizer.Tokenize(chosenFunction.GetExpression());
            var postfixTokens = Parser.ConvertToPostfix(tokens);

            Node root = CreateTree(postfixTokens);

            int leftOffset = 0;
            int rightOffset = 0;
            AddNodeToCanvas(root, 0);

            Node? left = root.GetLeft();
            Node? right = root.GetRight();
            while (left != null || right != null)
            {
                AddNodeToCanvas(left, leftOffset += 50);
                if (left != null)
                {
                    left = left.GetLeft();
                }

                AddNodeToCanvas(right, rightOffset -= 50);
                if (right != null)
                {
                    right = right.GetRight();
                }
            }
        }

        int leftOffset = 0;
        int rightOffset = 0;
        private void AddNodeToCanvas(Node? root, double offset)
        {
            if(root == null)
            {
                return;
            }

            var ellipse = new Ellipse();
            ellipse.Height = 40;
            ellipse.Width = 40;
            ellipse.Margin = new Thickness(-ellipse.Height / 2);
            ellipse.Fill = Brushes.Yellow;

            Grid container = new Grid();
            container.SetValue(Canvas.LeftProperty, TreeCanvas.Width / 2 + offset);

            var yOffest = 50d + offset;
            if (offset < 0)
            {
                yOffest += -offset * 2;
            }
            container.SetValue(Canvas.TopProperty, yOffest);
            container.Children.Add(ellipse);
            container.Children.Add(new TextBlock() { Text = root.GetValue() });

            TreeCanvas.Children.Add(container);
        }

        private void AddNodeToCanvas2(Node? root, double offset)
        {
            if (root == null)
            {
                return;
            }

            var ellipse = new Ellipse();
            ellipse.Height = 40;
            ellipse.Width = 40;
            ellipse.Margin = new Thickness(-ellipse.Height / 2);
            ellipse.Fill = Brushes.Yellow;

            Grid container = new Grid();
            container.SetValue(Canvas.LeftProperty, TreeCanvas.Width / 2 + offset);

            var yOffest = 50d + offset;
            if (offset < 0)
            {
                yOffest += -offset * 2;
            }
            container.SetValue(Canvas.TopProperty, yOffest);
            container.Children.Add(ellipse);
            container.Children.Add(new TextBlock() { Text = root.GetValue() });

            TreeCanvas.Children.Add(container);

            AddNodeToCanvas(root.GetLeft(), leftOffset += 100);
            AddNodeToCanvas(root.GetRight(), rightOffset -= 100);
        }

        private void CommandInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string input = Utility.TrimEnd(CommandInput.Text, '\n');
                string[] split = Utility.Split(input, '\r');

                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = Utility.TrimStart(split[i], '\n');
                }

                ProcessInput(split[split.Length - 1]);
            }
        }
    }
}
