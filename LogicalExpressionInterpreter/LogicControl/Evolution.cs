using LogicalExpressionInterpreter.BinaryTree;
using LogicalExpressionInterpreter.Parsing;
using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public static class Evolution
    {
        private static Random _random = new Random();
        private static List<string> _population = new();
        private static int _expressionVariableCount = 0;

        public static string ConstructBooleanExpression(string[,] truthTable)
        {
            _expressionVariableCount = truthTable.GetLength(1)-1;

            // Generate a random boolean expression and evaluate its fitness against the truth table
            string booleanExpression = GenerateRandomBooleanExpression(_expressionVariableCount);
            booleanExpression = Utility.TrimEnd(booleanExpression, ' ');

            int fitness = EvaluateFitness(booleanExpression, truthTable);

            // Create a population of candidate solutions and add the initial boolean expression to it
            _population.Add(booleanExpression);

            // Repeat the following steps until a boolean expression is found that perfectly matches the truth table
            while (fitness < truthTable.GetLength(0))
            {
                // Create a new generation of boolean expressions by applying evolutionary operators to the population
                List<string> newGeneration = ApplyEvolutionaryOperators(_population);

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

                // Add the new generation of expressions to the population
                _population.AddRange(newGeneration);
            }

            return booleanExpression;
        }

        private static int EvaluateFitness(string booleanExpression, string[,] truthTable)
        {
            int fitness = 0;
            for (int i = 0; i < truthTable.GetLength(0); i++)
            {
                var values = Utility.GetRowItemsWithoutLast(truthTable, i);
                var exp = Tokenizer.Tokenize(booleanExpression);

                var root = Tree.CreateTree(exp, values);
                var result = Tree.Evaluate(root);

                // Evaluate the boolean expression and compare the output to the expected result
                if (result == bool.Parse(truthTable[i, truthTable.GetLength(1) - 1]))
                {
                    fitness++;
                }
            }
            return fitness;
        }

        public static string GenerateRandomBooleanExpression(int operandCount)
        {
            string booleanExpression = "";

            for (int i = 0; i < operandCount; i++)
            {
                booleanExpression += _random.Next(0, 2) == 0 ? "a" : "a!";
                booleanExpression += " ";
            }

            int operatorCount = operandCount - 1;
            for (int i = 0; i < operatorCount; i++)
            {
                booleanExpression += _random.Next(0, 2) == 0 ? "&" : "|";
                booleanExpression += " ";
            }

            return booleanExpression;
        }

        public static List<string> ApplyEvolutionaryOperators(List<string> population)
        {
            List<string> newGeneration = new();

            // Generate a random number of mutated expressions
            int numMutations = _random.Next(0, 10);
            for (int i = 0; i < numMutations; i++)
            {
                // Select a random expression from the population to mutate
                string expression = SelectRandomExpression(population);

                // Apply mutation to the selected expression
                string mutatedExpression = Mutate(expression);

                if (Utility.GetCountOf(mutatedExpression, 'a') != _expressionVariableCount)
                {
                    mutatedExpression = Utility.TrimEnd(GenerateRandomBooleanExpression(_expressionVariableCount), ' ');
                }

                newGeneration.Add(mutatedExpression);
            }

            // Generate a random number of expressions by applying crossover to pairs of expressions
            int numCrossovers = _random.Next(0, 10);
            for (int i = 0; i < numCrossovers; i++)
            {
                // Select two random expressions from the population
                string expression1 = SelectRandomExpression(population);
                string expression2 = SelectRandomExpression(population);

                // Apply crossover to the selected expressions
                string crossoverExpression = Crossover(expression1, expression2);

                if (Utility.GetCountOf(crossoverExpression, 'a') != _expressionVariableCount)
                {
                    crossoverExpression = Utility.TrimEnd(GenerateRandomBooleanExpression(_expressionVariableCount), ' ');
                }

                newGeneration.Add(crossoverExpression);
            }

            return newGeneration;
        }

        // Apply mutation to a given boolean expression
        public static string Mutate(string booleanExpression)
        {
            // Choose a random part of the boolean expression to mutate
            int mutationIndex = _random.Next(booleanExpression.Length);
            char mutation = booleanExpression[mutationIndex];

            // Replace the chosen part of the boolean expression with a different value
            if (mutation == 'a' && booleanExpression[mutationIndex + 1] != '!')
            {
                return Utility.Substring(booleanExpression, 0, mutationIndex) + "a!" + Utility.Substring(booleanExpression, mutationIndex + 1);
            }
            else if (mutation == '!' && booleanExpression[mutationIndex - 1] == 'a')
            {
                return Utility.Substring(booleanExpression, 0, mutationIndex - 1) + "a" + Utility.Substring(booleanExpression, mutationIndex + 1);
            }
            else if (mutation == 'a' && booleanExpression[mutationIndex + 1] == '!')
            {
                return Utility.Substring(booleanExpression, 0, mutationIndex) + "a" + Utility.Substring(booleanExpression, mutationIndex + 1);
            }
            else if (mutation == '&')
            {
                return Utility.Substring(booleanExpression, 0, mutationIndex) + "|" + Utility.Substring(booleanExpression, mutationIndex + 1);
            }
            else if (mutation == '|')
            {
                return Utility.Substring(booleanExpression, 0, mutationIndex) + "&" + Utility.Substring(booleanExpression, mutationIndex + 1);
            }

            return booleanExpression;
        }

        // Apply crossover to two given boolean expressions
        public static string Crossover(string booleanExpression1, string booleanExpression2)
        {
            // Choose a random crossover point
            int crossoverPoint = _random.Next(1, booleanExpression1.Length - 1 - Utility.GetCountOf(booleanExpression1, '!'));

            // Combine the two boolean expressions at the crossover point
            if (booleanExpression1[crossoverPoint - 1] != ' ' || booleanExpression2[crossoverPoint - 1] != ' ')
            {
                int choice = _random.Next(0, 2);
                switch (choice)
                {
                    case 0: return booleanExpression1;
                    case 1: return booleanExpression2;
                }
            }

            string crossoverExpression = Utility.Substring(booleanExpression1, 0, crossoverPoint) + Utility.Substring(booleanExpression2, crossoverPoint);
            return crossoverExpression;
        }

        public static string SelectRandomExpression(List<string> expressions)
        {
            int index = _random.Next(expressions.Count);
            return expressions[index];
        }
    }
}
