using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Parser
{
    public class NwdParser
    {
        /// <summary>
        /// gets the value for the key specified, returns an
        /// empty string ("") otherwise
        /// </summary>
        /// <param name="nestedKey"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Extract(String nestedKey, String input)
        {
            if (ValidateNestedKey(nestedKey) && Validate(input))
            {

                Stack<string> keyStack = GetInvertedKeyStack(nestedKey);

                string currentNestedLevelString = input;

                while (keyStack.Count > 0)
                {

                    string key = keyStack.Pop();

                    currentNestedLevelString =
                            ExtractOne(key, currentNestedLevelString);
                }

                return currentNestedLevelString;

            }
            else
            {

                return "";
            }
        }

        public static bool ValidateNestedKey(string nestedKey)
        {
            return ValidateNonEmptyKeyNodes(nestedKey) &&
                ValidateForwardSlashKeyNotation(nestedKey) &&
                ValidateNonEmptyKey(nestedKey);
        }

        public static bool ValidateForwardSlashKeyNotation(string nestedKey)
        {
            return !nestedKey.Contains(@"\");
        }

        public static bool ValidateNonEmptyKeyNodes(string nestedKey)
        {
            return !nestedKey.Contains("//");
        }

        public static bool ValidateNonEmptyKey(string nestedKey)
        {
            return nestedKey.Replace("/", "").Trim().Count() > 0;
        }

        /// <summary>
        /// returns true if a string is a valid fragment
        ///  
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool Validate(string input)
        {
            return ValidateOpenKeyFormat(input) &&
                    ValidateMatchBraces(input);
        }

        public static bool ValidateOpenKeyFormat(string input)
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

        /// <summary>
        /// A string fragment is atomic if all mixed content is contained 
        /// within one or more keyVal structure(s) (eg. thisExample={is atomic}).
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsAtomic(string input)
        {
            string temp = input;

            while (temp.Length > 0 && StartsWithKeyValTag(temp))
            {
                string key = GetFirstKey(temp);
                temp = TrimKeyVal(key, temp);
            }

            return temp.Trim().Length == 0;
        }

        /// <summary>
        /// returns first key found sequentially, irrespective of 
        /// node depth, whether input is atomic or mixed content.
        /// (eg. input of "item={something}" will return "item"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetFirstKey(string input)
        {
            if (!input.Contains("={"))
            {
                return null;
            }

            string firstSegment = input.Split(new string[] { "={" },
                StringSplitOptions.None)[0];

            string lastWord = firstSegment.Split(' ').Last();

            return lastWord.Trim();
        }

        private static bool StartsWithKeyValTag(string input)
        {
            string trimmed = input.Trim();

            if (trimmed.IndexOf("={") < 0)
            {
                return false;
            }

            if (trimmed.IndexOf(" ") < 0)
            {
                return true;
            }

            return trimmed.IndexOf("={") < trimmed.IndexOf(" ");
        }

        /// <summary>
        /// Trims first occurance of key and its internal value from the given
        /// string. Extraneous whitespace is also trimmed from the copy
        /// that is returned. Original string is unchanged. 
        /// 
        /// output example: input of "item={something} item2={something else}"
        /// will return "item2={something else}"
        /// 
        /// NOTE: this will
        /// strip tags internal to mixed content, so use with caution. Primarily
        /// intended to be used in conjuntion with GetFirstKey() 
        /// to process and remove keyVals one by one.
        /// </summary>
        /// <param name="keyValPair">the key of the keyVal to trim</param>
        /// <param name="input">the string to trim the first instance from</param>
        /// <returns></returns>
        public static string TrimKeyVal(string key, string input)
        {
            string value = Extract(key, input);
            string keyVal = key + "={" + value + "}";
            string temp = input.ReplaceFirst(keyVal, " ");

            //clear any double spaces that result
            while (temp.Contains("  "))
            {
                temp = temp.Replace("  ", " ");
            }

            return temp.Trim();
        }

        public static bool ValidateMatchBraces(string input)
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

        public static string ExtractOne(string key, string input)
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

        public static Stack<string> GetInvertedKeyStack(string nestedKey)
        {

            Stack<string> s = new Stack<string>();

            string[] keys = nestedKey.Split('/');

            for (int i = keys.Length - 1; i >= 0; i--)
            {

                s.Push(keys[i]);
            }

            return s;
        }

    }
}
