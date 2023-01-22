using LogicalExpressionInterpreter.LogicControl;
using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.BinaryTree
{
    public static class Tree
    {
        public static bool Evaluate(Node? root)
        {
            if (root == null)
            {
                return false;
            }

            if (root.GetLeft() == null && root.GetRight() == null)
            {
                return bool.Parse(root.GetValue()) == true;
            }

            bool left = Evaluate(root.GetLeft());
            bool right = Evaluate(root.GetRight());

            switch (root.GetValue())
            {
                case "&&": return left && right;
                case "||": return left || right;
                case "!": return !left;
            }

            return false;
        }

        private static ObjectStack<string[]> nestedBooleans = new();
        private static List<string> bools = new();
        private static void FillTree(List<Token> tokens, string[] boolValues, out bool error)
        {
            int index = 0;
            error = false;

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == Token.TokenType.NESTED_FUNCTION)
                {
                    var nestedFunction = LogicController.ChooseFunction(Utility.Split(tokens[i].Value, '(')[0]);

                    if(nestedFunction == null)
                    {
                        error = true;
                        return;
                    }

                    for (int k = 0; k < nestedFunction.GetOperands().Count; k++)
                    {
                        bools.Add(boolValues[index]);
                        index++;
                    }
                    nestedBooleans.Push(bools.ToArray());
                    bools.Clear();
                }
                else if (tokens[i].Type == Token.TokenType.LITERAL)
                {
                    tokens[i].Value = boolValues[index];
                    index++;
                }
            }

            nestedBooleans = Utility.ReverseStack(nestedBooleans);
            bools.Clear();
        }

        public static Node? CreateTree(List<Token> tokens, string[] boolValues)
        {
            FillTree(tokens, boolValues, out bool error);

            if(error == true)
            {
                return null;
            }

            ObjectStack<Node> nodes = new();

            for (int i = 0; i < tokens.Count; i++)
            {
                switch (tokens[i].Type)
                {
                    case Token.TokenType.NESTED_FUNCTION:
                        {
                            var nestedFunction = LogicController.ChooseFunction(Utility.Split(tokens[i].Value, '(')[0]);
                            var nestedBoolValues = nestedBooleans.Pop();

                            if (nestedFunction.ContainsResult(Utility.Concat(nestedBoolValues)))
                            {
                                nodes.Push(new Node(nestedFunction.GetResult(Utility.Concat(nestedBoolValues)).ToString()));
                                break;
                            }

                            var nestedNode = CreateTree(Parser.ConvertToPostfix(nestedFunction.GetTokens()), nestedBoolValues);
                            var result = Evaluate(nestedNode);
                            nestedFunction.AddResult(Utility.Concat(nestedBoolValues), result);
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

        public static Node? CreateTree(List<Token> tokens)
        {
            ObjectStack<Node> nodes = new();

            for (int i = 0; i < tokens.Count; i++)
            {
                switch (tokens[i].Type)
                {
                    case Token.TokenType.NESTED_FUNCTION:
                        {
                            var nestedFunction = LogicController.ChooseFunction(Utility.Split(tokens[i].Value, '(')[0]);

                            if(nestedFunction == null)
                            {
                                return null;
                            }

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

        public static int TreeDepth(Node? root)
        {
            if (root == null)
            {
                return 0;
            }

            int left = TreeDepth(root.GetLeft());
            int right = TreeDepth(root.GetRight());

            return Math.Max(left, right) + 1;
        }
    }
}
