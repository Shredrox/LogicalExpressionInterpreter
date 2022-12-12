using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public static class DataControl
    {
        public static void SaveToFile(List<LogicFunction>? userFunctions, string path)
        {
            if(userFunctions == null)
            {
                return;
            }

            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i = 0; i < userFunctions.Count; i++)
                {
                    sw.WriteLine(userFunctions[i].GetCombinedName() + ":" + userFunctions[i].GetExpression());
                }
            }
        }

        public static List<LogicFunction> LoadFromFile(string path)
        {
            string[] fileLines = File.ReadAllLines(path);
            List<LogicFunction> loadedFunctions = new();

            for (int i = 0; i < fileLines.Length; i++)
            {
                string[] values = Utility.Split(fileLines[i], ':');
                string[] splitName = Utility.Split(values[0], '(');
                string name = splitName[0];
                string expression = Utility.TrimStart(values[1], ' ');

                var loadedFunction = new LogicFunction(name, expression, values[0]);
                loadedFunctions.Add(loadedFunction);
            }

            return loadedFunctions;
        }
    }
}
