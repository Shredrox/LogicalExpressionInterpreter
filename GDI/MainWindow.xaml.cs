using System;
using System.Collections.Generic;
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
                    && Utility.ToUpper(inputSplit[0]) != "EXIT")
            {
                MessageBox.Show("Invalid Command.");
                return;
            }

            switch (Utility.ToUpper(inputSplit[0]))
            {
                case "DEFINE": 
                    {
                        LogicController.AddFunction(inputSplit[1], out string errorMsg);

                        if(errorMsg != "")
                        {
                            MessageBox.Show(errorMsg);
                        }
                        else
                        {
                            LogicController.SaveFunctions();
                        }

                        break;
                    } 
                case "REMOVE": LogicController.RemoveFunction(inputSplit[1]); LogicController.SaveFunctions(); break;
                case "PRINTALL": PrintFunctions(); break;
                case "SOLVE": 
                    {
                        string result = LogicController.SolveFunction(inputSplit[1], out string errorMsg); 

                        if(errorMsg != "")
                        {
                            MessageBox.Show(errorMsg);
                        }
                        else
                        {
                            TextDisplay.Inlines.Add(result);
                        }

                        break;
                    } 
                case "ALL":
                    {
                        LogicController.CreateTruthTable(inputSplit[1], out LogicFunction? functionWithCreatedTable);
                        if (functionWithCreatedTable == null)
                        {
                            MessageBox.Show("This function doesn't exist.");
                            return;
                        }

                        PrintTruthTable(functionWithCreatedTable);
                        break;
                    }
                    
                case "DISPLAY": DisplayTree(inputSplit[1]); break;
                case "EXIT": LogicController.SaveFunctions(); this.Close(); return;
                default: MessageBox.Show("Invalid Command."); break;
            }
        }

        static bool containsFIND = false;
        static List<string> findCommand = new();
        public static void CheckForFindCommand(string[] input)
        {
            if (Utility.ToUpper(Utility.Split(input[0], ' ')[0]) == "FIND")
            {
                containsFIND = true;
                return;
            }

            containsFIND = false;
        }

        //Window functions
        private void CommandInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
            {
                return;
            }

            if (Utility.StringIsNullOrEmpty(CommandInput.Text))
            {
                return;
            }

            string input = Utility.TrimEnd(CommandInput.Text, '\n');
            string[] split = Utility.Split(input, '\r');

            for (int i = 0; i < split.Length; i++)
            {
                if (Utility.StringIsNullOrEmpty(split[i]))
                {
                    continue;
                }

                split[i] = Utility.TrimStart(split[i], '\n');
            }

            CheckForFindCommand(split);

            if (containsFIND)
            {
                CommandInput.AcceptsReturn = true;

                if (split[^1] == null)
                {
                    for (int i = 0; i < split.Length; i++)
                    {
                        if (Utility.StringIsNullOrEmpty(split[i]))
                        {
                            continue;
                        }

                        findCommand.Add(split[i]);
                    }

                    string result = LogicController.FindFunction(findCommand, out string errorMsg);
                    if (errorMsg != "")
                    {
                        MessageBox.Show(errorMsg);
                    }
                    else
                    {
                        TextDisplay.Inlines.Add(result);
                    }
                    
                    findCommand.Clear();
                    containsFIND = false;
                    CommandInput.AcceptsReturn = false;
                    CommandInput.Clear();
                }

                return;
            }

            ProcessInput(split[^1]);
            CommandInput.Clear();
        }

        public void PrintFunctions()
        {
            var functionsInfo = LogicController.GetFunctionsInfo();
            if (functionsInfo == null)
            {
                MessageBox.Show("No added functions.");
                return;
            }

            for (int i = 0; i < functionsInfo.Count; i++)
            {
                TextDisplay.Inlines.Add(functionsInfo[i]);
            }
        }

        public void PrintTruthTable(LogicFunction logicFunction)
        {
            if(logicFunction.GetTruthTable() == null)
            {
                MessageBox.Show("Truth table is null.");
                return;
            }

            if (App.Current.Windows.Count > 2)
            {
                foreach (Window win in App.Current.Windows)
                {
                    if (win != this)
                    {
                        win.Close();
                    }  
                }
            }

            List<string> lines = new();
            lines.Add(logicFunction.GetCombinedName() + ": " + logicFunction.GetExpression() + "\n\n");
            lines.Add("Truth Table: \n");

            string line = "";
            for (int i = 0; i < logicFunction.GetOperands().Count; i++)
            {
                line += logicFunction.GetOperands()[i] + "\t| ";
            }
            line += "Result\t|";

            lines.Add(line + "\n");

            line = "";

            for (int row = 0; row < logicFunction.GetTruthTable().GetLength(1); row++)
            {
                for (int col = 0; col < logicFunction.GetTruthTable().GetLength(0); col++)
                {
                    line += logicFunction.GetTruthTable()[col, row] + "\t| ";
                }

                lines.Add(line + "\n"); 
                line = "";
            }

            string table = "";
            for (int i = 0; i < lines.Count; i++)
            {
                table += lines[i];
            }

            TruthTableWindow truthTableWindow = new TruthTableWindow(table);
            truthTableWindow.Show();
        }

        #region ExpressionTreeDisplay
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
            if(root == null)
            {
                return;
            }

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
        #endregion

        #region WindowMethods
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
        #endregion
    }
}
