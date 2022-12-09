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

        public static string[] Split(string input, char separator, int count)
        {
            int counter = 0;
            string temp = "";

            string[] splitString = new string[count];

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == separator && counter != count-1)
                {
                    splitString[counter] = temp;
                    counter++;
                    temp = "";
                    continue;
                }

                temp += input[i];
            }

            splitString[counter] = temp;

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

        public static string TrimEnd(string input, char trimChar)
        {
            string trimmedStringReversed = "";
            string trimmedString = "";
            bool passedWhitespaces = false;

            for (int i = input.Length-1; i >= 0; i--)
            {
                if (input[i] == trimChar && !passedWhitespaces)
                {
                    continue;
                }
                else
                {
                    passedWhitespaces = true;
                }

                trimmedStringReversed += input[i];
            }

            for (int i = trimmedStringReversed.Length - 1 ; i >= 0; i--)
            {
                trimmedString += trimmedStringReversed[i];
            }

            return trimmedString;
        }

        public static string TrimStart(string input, char trimChar)
        {
            string trimmedString = "";
            bool passedWhitespaces = false;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == trimChar && !passedWhitespaces)
                {
                    continue;
                }
                else
                {
                    passedWhitespaces = true;
                }

                trimmedString += input[i];
            }

            return trimmedString;
        }

        public static string TrimEndToPenultimate(string input, char trimChar)
        {
            string trimmedStringReversed = "";
            string trimmedString = "";
            bool passedWhitespaces = false;

            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (input[i] == trimChar && !passedWhitespaces)
                {
                    if (input[i] == trimChar && input[i - 1] != trimChar && i - 1 >= 0)
                    {
                        trimmedStringReversed += input[i];
                    }
                    continue;
                }
                else
                {
                    passedWhitespaces = true;
                }

                trimmedStringReversed += input[i];
            }

            for (int i = trimmedStringReversed.Length - 1; i >= 0; i--)
            {
                trimmedString += trimmedStringReversed[i];
            }

            return trimmedString;
        }

        public static string TrimEndToPenultimateValue(string input, char trimChar)
        {
            string trimmedStringReversed = "";
            string trimmedString = "";
            bool passedWhitespaces = false;

            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (input[i] == trimChar && !passedWhitespaces)
                {
                    if (input[i] == trimChar && input[i - 1] != trimChar && i - 1 >= 0)
                    {
                        break;
                    }
                    trimmedStringReversed += input[i];
                    continue;
                }
                else
                {
                    passedWhitespaces = true;
                }
            }

            for (int i = trimmedStringReversed.Length - 1; i >= 0; i--)
            {
                trimmedString += trimmedStringReversed[i];
            }

            return trimmedString;
        }

        public static string Concat(string[] strings)
        {
            string result = "";
            for (int i = 0; i < strings.Length; i++)
            {
                result += strings[i];
            }

            return result;
        }

        public static string ConcatWithSpaces(List<string> strings)
        {
            string result = "";
            for (int i = 0; i < strings.Count; i++)
            {
                if (i + 1 == strings.Count)
                {
                    result += strings[i];
                    break;
                }
                result += strings[i] + " ";
            }

            return result;
        }

        public static string ConcatWithSpaces(string[] strings)
        {
            string result = "";
            for (int i = 0; i < strings.Length; i++)
            {
                if (i+1 == strings.Length)
                {
                    result += strings[i];
                    break;
                }
                result += strings[i] + " ";
            }

            return result;
        }

        public static bool Contains(string input, char c)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == c)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsMoreThanOneLetter(string input)
        {
            int counter = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if(counter > 1)
                {
                    return true;
                }
                if ((input[i] >= 97 && input[i] <= 122) || (input[i] >= 65 && input[i] <= 90))
                {
                    counter++;
                }
            }

            return false;
        }

        public static string TrimStartValue(string input, char trimChar)
        {
            string trimmedString = "";
            bool passedWhitespaces = false;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == trimChar && !passedWhitespaces)
                {
                    trimmedString += input[i];
                    continue;
                }
                else
                {
                    passedWhitespaces = true;
                }
            }

            return trimmedString;
        }

        public static string TrimEndValue(string input, char trimChar)
        {
            string trimmedStringReversed = "";
            string trimmedString = "";
            bool passedWhitespaces = false;

            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (input[i] == trimChar && !passedWhitespaces)
                {
                    trimmedStringReversed += input[i];
                    continue;
                }
                else
                {
                    passedWhitespaces = true;
                }
            }

            for (int i = trimmedStringReversed.Length - 1; i >= 0; i--)
            {
                trimmedString += trimmedStringReversed[i];
            }

            return trimmedString;
        }

        public static string ToUpper(string input)
        {
            string upper = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] >= 'a' && input[i] <= 'z')
                {
                    upper += Convert.ToChar(input[i] - 32);
                    continue;
                }

                upper += input[i];
            }

            return upper;
        }

        public static string Substring(string input, int startIndex)
        {
            string substring = "";
            for (int i = startIndex; i < input.Length; i++)
            {
                substring += input[i];
            }

            return substring;
        }

        public static string Substring(string input, int startIndex, int length)
        {
            string substring = "";
            for (int i = startIndex; i < length; i++)
            {
                substring += input[i];
            }

            return substring;
        }

        public static bool StringIsNullOrEmpty(string input)
        {
            if(input == "" || input == " " || input == null)
            {
                return true;
            }

            return false;
        }

        public static int GetCountOf(string input, char c) 
        {
            int counter = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == c)
                {
                    counter++;
                }
            }

            return counter;
        }

        public static int IntPower(int baseNum, int power)
        {
            int result = baseNum;
            for (int i = 1; i < power; i++)
            {
                result *= baseNum;
            }

            return result;
        }

        public static string[]? CheckBoolInput(string[] input)
        {
            string[] boolValues = input;
            for (int i = 0; i < boolValues.Length; i++)
            {
                boolValues[i] = Utility.TrimStart(boolValues[i], ' ');

                if (!bool.TryParse(boolValues[i], out _))
                {
                    return null;
                }
            }

            return boolValues;
        }

        public static string InvertOperand(string operand)
        {
            if(Contains(operand,'!'))
            {
                return operand[0].ToString();
            }

            return operand + "!";
        }

        public static string[] GetRowItemsWithoutLast(string[,] input, int rowIndex)
        {
            string[] row = new string[input.GetLength(1) - 1];

            for (int i = 0; i < input.GetLength(1) - 1; i++)
            {
                row[i] = input[rowIndex, i];
            }

            return row;
        }

        public static ObjectStack<string[]> ReverseStack(ObjectStack<string[]> values)
        {
            ObjectStack<string[]> reversed = new();

            while (values.Count() != 0)
            {
                reversed.Push(values.Pop());
            }

            return reversed;
        }
    }
}
