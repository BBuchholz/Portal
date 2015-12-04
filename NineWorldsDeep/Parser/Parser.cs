using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Parser
{
    //TODO: this is cut and paste java from NwdParserLibrary, need to port to c#
    public class Parser
    {
        public string Extract(String nestedKey, String input)
        {

            if (validateNestedKey(nestedKey) && validate(input))
            {

                Stack<string> keyStack = GetInvertedKeyStack(nestedKey);

                string currentNestedLevelString = input;

                while (keyStack.Count > 0)
                {

                    string key = keyStack.Pop();

                    currentNestedLevelString =
                            extractOne(key, currentNestedLevelString);
                }

                return currentNestedLevelString;

            }
            else
            {

                return null;
            }
        }

        public string extractOne(string key, string input)
        {

            string openKey = key + "={";

            int startIndex = input.IndexOf(openKey);

            if (startIndex > -1)
            {

                int innerContentStartIndex =
                    startIndex + openKey.Length;
                int innerContentEndIndex =
                        input.Length - 1; //just a temp value

                int nestingLevel = 0;
                bool closed = false;

                for (int i = startIndex; !closed; i++)
                {

                    if ('{'.CompareTo(input[i]) == 0)
                    {

                        nestingLevel++;
                    }

                    if ('}'.CompareTo(input[i]) == 0)
                    {

                        nestingLevel--;

                        if (nestingLevel == 0)
                        {
                            closed = true;
                            innerContentEndIndex = i;
                        }
                    }
                }

                return input.Substring(innerContentStartIndex,
                                       innerContentEndIndex - innerContentStartIndex);
            }

            return null;
        }

        public Stack<string> GetInvertedKeyStack(string nestedKey)
        {

            Stack<string> s = new Stack<string>();

            string[] keys = nestedKey.Split('/');

            for (int i = keys.Length - 1; i >= 0; i--)
            {

                s.Push(keys[i]);
            }

            return s;
        }

        public bool validateNestedKey(string nestedKey)
        {

            return validateNonEmptyKeyNodes(nestedKey);
        }

        public bool validateNonEmptyKeyNodes(string nestedKey)
        {

            return !nestedKey.Contains("//");
        }

        public bool validate(string input)
        {
            return validateOpenKeyFormat(input) &&
                    validateMatchBraces(input);
        }

        public bool validateOpenKeyFormat(string input)
        {

            List<int> openBraceIndexes = new List<int>();

            for (int i = 0; i < input.Length; i++)
            {

                char c = input[i];

                if (c.CompareTo('{') == 0)
                {
                    openBraceIndexes.Add(i);
                }
            }

            bool valid = true;

            foreach (int i in openBraceIndexes)
            {

                if (i < 2)
                {
                    return false;
                }

                char alphaNumeric = input[i - 2];
                char equals = input[i - 1];

                if (!Char.IsLetterOrDigit(alphaNumeric) ||
                   !(equals.CompareTo('=') == 0))
                {

                    valid = false;
                }
            }

            return valid;
        }

        public bool validateMatchBraces(string input)
        {

            int openBracesCount = 0;
            int closingBracesCount = 0;

            foreach (char c in input)
            {

                if (c.CompareTo('{') == 0)
                    openBracesCount++;
                if (c.CompareTo('}') == 0)
                    closingBracesCount++;
            }

            return openBracesCount == closingBracesCount;
        }
    }
}
