using LogicalExpressionInterpreter.LogicControl;
using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.BinaryTree
{
    public static class Tree
    {
        public static bool Evaluate(Node root)
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
                case "True":
                case "T":
                    return true;
                case "False":
                case "F":
                    return false;
            }

            return false;
        }

        private static List<string> nestedBools = new();
        private static void FillTree(List<Token> tokens, string[] boolValues)
        {
            int index = 0;

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == Token.TokenType.NESTED_FUNCTION)
                {
                    var nestedFunction = LogicController.ChooseFunction(Utility.Split(tokens[i].Value, '(')[0]);
                    for (int k = 0; k < nestedFunction.GetOperands().Count; k++)
                    {
                        nestedBools.Add(boolValues[index]);
                        index++;
                    }
                }
                else if (tokens[i].Type == Token.TokenType.LITERAL)
                {
                    tokens[i].Value = boolValues[index];
                    index++;
                }
            }
        }

        public static Node CreateTree(List<Token> tokens, string[] boolValues)
        {
            FillTree(tokens, boolValues);
            //solve func3(true,true,false,false)
            ObjectStack<Node> nodes = new();

            for (int i = 0; i < tokens.Count; i++)
            {
                switch (tokens[i].Type)
                {
                    case Token.TokenType.NESTED_FUNCTION:
                        {
                            var nestedFunction = LogicController.ChooseFunction(Utility.Split(tokens[i].Value, '(')[0]);
                            Console.WriteLine();
                            var nestedNode = CreateTree(Parser.ConvertToPostfix(nestedFunction.GetTokens()), nestedBools.ToArray());
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
    }
}
