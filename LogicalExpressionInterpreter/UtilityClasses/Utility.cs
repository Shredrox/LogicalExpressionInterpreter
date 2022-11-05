namespace LogicalExpressionInterpreter.UtilityClasses
{
    public class Utility
    {
        public static string[] Split(string input, char separator)
        {
            int counter = 1;
            string temp = "";

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == separator)
                {
                    counter++;
                }
            }

            string[] splitString = new string[counter];

            counter = -1;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != separator)
                {
                    temp += input[i];
                }

                if (input[i] == separator || i == input.Length - 1)
                {
                    counter++;
                    splitString[counter] = temp;
                    temp = "";
                }
            }

            return splitString;
        }

        public static int SplitSize(string input, char separator)
        {
            int counter = 1;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == separator)
                {
                    counter++;
                }
            }

            return counter;
        }
    }
}
