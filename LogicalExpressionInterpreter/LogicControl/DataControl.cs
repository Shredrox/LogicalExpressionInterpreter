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
                    sw.WriteLine(userFunctions[i].GetName() + ":" + userFunctions[i].GetExpression());
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

                var loadedFunction = new LogicFunction(values[0], values[1]);
                loadedFunctions.Add(loadedFunction);
            }

            return loadedFunctions;
        }
    }
}
