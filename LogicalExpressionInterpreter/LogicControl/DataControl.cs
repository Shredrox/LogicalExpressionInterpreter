using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public static class DataControl
    {
        public static void SaveToFile(List<LogicFunction> userFunctions, string path)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i = 0; i < userFunctions.Count; i++)
                {
                    sw.Write(userFunctions[i].GetName() + ":" + userFunctions[i].GetExpression() + "," + userFunctions[i].GetID());

                    if (userFunctions[i].GetNestedFunctions().Count != 0)
                    {
                        for (int k = 0; k < userFunctions[i].GetNestedFunctions().Count; k++)
                        {
                            sw.Write("," + userFunctions[i].GetNestedFunctions()[k].GetID());
                        }
                    }
                    sw.WriteLine();
                }
            }
        }

        public static List<LogicFunction>? LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            string[] fileLines = File.ReadAllLines(path);
            List<LogicFunction> loadedFunctions = new();

            for (int i = 0; i < fileLines.Length; i++)
            {
                string[] values = Utility.Split(fileLines[i], ':');
                string[] valuesSplit = Utility.Split(values[1], ',');

                var loadedFunction = new LogicFunction(values[0], valuesSplit[0]);
                loadedFunction.SetID(valuesSplit[1]);

                if(valuesSplit.Length > 2)
                {
                    for (int k = 2; k < valuesSplit.Length; k++)
                    {

                    }
                }
                

                loadedFunctions.Add(loadedFunction);
            }

            return loadedFunctions;
        }
    }
}
