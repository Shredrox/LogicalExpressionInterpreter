using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public static class DataControl
    {
        public static void SaveToFile(DynamicArray<LogicFunction> userFunctions, string path)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i = 0; i < userFunctions.Count; i++)
                {
                    sw.WriteLine(userFunctions[i].GetExpression());
                }
            }
        }

        public static DynamicArray<LogicFunction>? LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            string[] fileLines = File.ReadAllLines(path);
            DynamicArray<LogicFunction> loadedFunctions = new();

            for (int i = 0; i < fileLines.Length; i++)
            {
                loadedFunctions.Add(new LogicFunction(fileLines[i]));
            }

            return loadedFunctions;
        }
    }
}
