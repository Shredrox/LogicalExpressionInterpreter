using LogicalExpressionInterpreter.BinaryTree;
using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;
using System.Diagnostics;

namespace LogicalExpressionInterpreter.LogicControl
{
    public static class Evolution
    {
        private static Random _random = new Random();
        private static List<List<string>> _population = new();
        private static int _expressionVariableCount = 0;
        private static Stopwatch _stopwatch = new Stopwatch();

        public static string ConstructBooleanExpression(string[,] truthTable)
        {
            _stopwatch.Start();
            _expressionVariableCount = truthTable.GetLength(1) - 1;

            List<string> booleanExpression = GenerateRandomBooleanExpression(_expressionVariableCount);

            int fitness = EvaluateFitness(booleanExpression, truthTable);

            _population.Add(booleanExpression);

            while (fitness < truthTable.GetLength(0) && _stopwatch.Elapsed < TimeSpan.FromSeconds(5))
            {
                // Create a new generation of boolean expressions by applying evolutionary operators to the population
                List<List<string>> newGeneration = ApplyEvolutionaryOperators(_population);

                // Evaluate each of the new boolean expressions against the truth table to determine their fitness
                for (int i = 0; i < newGeneration.Count; i++)
                {
                    int expressionFitness = EvaluateFitness(newGeneration[i], truthTable);
                    if (expressionFitness > fitness)
                    {
                        booleanExpression = newGeneration[i];
                        fitness = expressionFitness;
                    }
                }

                _population.AddRange(newGeneration);
            }

            if(_stopwatch.Elapsed >= TimeSpan.FromSeconds(5))
            {
                _population.Clear();
                return "";
            }

            return Utility.ConcatWithSpaces(booleanExpression);
        }

        private static int EvaluateFitness(List<string> booleanExpression, string[,] truthTable)
        {
            int fitness = 0;
            for (int i = 0; i < truthTable.GetLength(0); i++)
            {
                var tableValues = Utility.GetRowItemsWithoutLast(truthTable, i);
                string expression = Utility.ConcatWithSpaces(booleanExpression);

                var tokens = Tokenizer.Tokenize(expression);
                var postfixTokens = Parser.ConvertToPostfix(tokens);

                var root = Tree.CreateTree(postfixTokens, tableValues);
                var result = Tree.Evaluate(root);

                // Evaluate the boolean expression and compare the output to the expected result
                if (result == bool.Parse(truthTable[i, truthTable.GetLength(1) - 1]))
                {
                    fitness++;
                }
            }

            return fitness;
        }

        private static List<string> GenerateRandomBooleanExpression(int operandCount)
        {
            List<string> booleanExpression = new();

            int letterValue = 97;

            for (int i = 0; i < operandCount; i++)
            {
                char operand = (char)letterValue;
                booleanExpression.Add(_random.Next(0, 2) == 0 ? operand.ToString() : "!" + operand);
                if (i != operandCount - 1)
                {
                    booleanExpression.Add(_random.Next(0, 2) == 0 ? "&&" : "||");
                }
                
                letterValue++;
            }

            return booleanExpression;
        }

        private static List<string> SelectRandomExpression()
        {
            int index = _random.Next(_population.Count);
            return _population[index];
        }

        private static List<List<string>> ApplyEvolutionaryOperators(List<List<string>> population)
        {
            List<List<string>> newGeneration = new();

            // Generate a random number of mutated expressions
            int numMutations = _random.Next(0, 10);
            for (int i = 0; i < numMutations; i++)
            {
                List<string> expression = SelectRandomExpression();
                Mutate(expression);

                newGeneration.Add(expression);
            }

            // Generate a random number of expressions by applying crossover to pairs of expressions
            int numCrossovers = _random.Next(0, 10);
            for (int i = 0; i < numCrossovers; i++)
            {
                List<string> expression1 = SelectRandomExpression();
                List<string> expression2 = SelectRandomExpression();

                List<string> crossoverExpression = Crossover(expression1, expression2);

                newGeneration.Add(crossoverExpression);
            }

            return newGeneration;
        }

        private static void Mutate(List<string> booleanExpression)
        {
            // Choose a random part of the boolean expression to mutate
            int mutationIndex = _random.Next(booleanExpression.Count);
            string mutation = booleanExpression[mutationIndex];

            // Replace the chosen part of the boolean expression with a different value
            switch (mutation)
            {
                case "&&": booleanExpression[mutationIndex] = "||"; return;
                case "||": booleanExpression[mutationIndex] = "&&"; return;
                default: booleanExpression[mutationIndex] = Utility.InvertOperand(mutation); return;
            }
        }

        private static List<string> Crossover(List<string> booleanExpression1, List<string> booleanExpression2)
        {
            int crossoverPoint = _random.Next(1, booleanExpression1.Count - 1);
            List<string> crossoverExpression = new();

            for (int i = 0; i < crossoverPoint; i++)
            {
                crossoverExpression.Add(booleanExpression1[i]);
            }

            for (int i = crossoverPoint; i < booleanExpression2.Count; i++)
            {
                crossoverExpression.Add(booleanExpression2[i]);
            }

            return crossoverExpression;
        }
    }
}
