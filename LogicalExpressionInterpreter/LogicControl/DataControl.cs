using LogicalExpressionInterpreter.UtilityClasses;

namespace LogicalExpressionInterpreter.LogicControl
{
    public static class DataControl
    {
        public static void SaveToFile(DynamicArray<string> userFunctions, string path)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                for (int i = 0; i < userFunctions.Count; i++)
                {
                    sw.WriteLine(userFunctions[i]);
                }
            }
        }

        public static DynamicArray<string>? LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            string[] fileLines = File.ReadAllLines(path);
            DynamicArray<string> loadedFunctions = new();

            for (int i = 0; i < fileLines.Length; i++)
            {
                loadedFunctions.Add(fileLines[i]);
            }

            return loadedFunctions;
        }
    }
}
