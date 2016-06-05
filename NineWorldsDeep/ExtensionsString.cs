using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NineWorldsDeep
{
    /// <summary>
    /// This class holds extension methods for string
    /// similarity ranking found at:
    /// TODO: LICENSE NOTES
    /// http://stackoverflow.com/questions/653157/a-better-similarity-ranking-algorithm-for-variable-length-strings
    /// 
    /// code from the site is marked with the region
    /// "code from stackoverflow"
    /// </summary>
    public static class ExtensionsString
    {
        #region "code from stackoverflow"

        /// <summary>
        /// Replaces first instance of search string with replace string
        /// </summary>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            //TODO: LICENSE NOTES
            //Source: http://stackoverflow.com/questions/141045/how-do-i-replace-the-first-instance-of-a-string-in-net

            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        /// <summary>
        /// performs a Contains(value) on string s for all values in containsParams.
        /// returns true if any of them are contained in given string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="containsParams"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string s, IEnumerable<string> containsParams)
        {
            foreach(string parm in containsParams)
            {
                if (s.Contains(parm))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Replaces last instance of search string with replace string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplaceLast(this string text, string search, string replace)
        {
            int pos = text.LastIndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        /// <summary>
        /// This class implements string comparison algorithm
        /// based on character pair similarity
        /// </summary>
        public static double PercentMatchTo(this string str1, string str2)
        {//TODO: LICENSE NOTES
            //Source: http://www.catalysoft.com/articles/StrikeAMatch.html

            List<string> pairs1 = WordLetterPairs(str1.ToUpper());
            List<string> pairs2 = WordLetterPairs(str2.ToUpper());

            int intersection = 0;
            int union = pairs1.Count + pairs2.Count;

            for (int i = 0; i < pairs1.Count; i++)
            {
                for (int j = 0; j < pairs2.Count; j++)
                {
                    if (pairs1[i] == pairs2[j])
                    {
                        intersection++;
                        pairs2.RemoveAt(j);//Must remove the match to prevent "GGGG" from appearing to match "GG" with 100% success

                        break;
                    }
                }
            }

            return (2.0 * intersection) / union;
        }

        /// <summary>
        /// Gets all letter pairs for each
        /// individual word in the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static List<string> WordLetterPairs(string str)
        {
            List<string> AllPairs = new List<string>();

            // Tokenize the string and put the tokens/words into an array
            string[] Words = Regex.Split(str, @"\s");

            // For each word
            for (int w = 0; w < Words.Length; w++)
            {
                if (!string.IsNullOrEmpty(Words[w]))
                {
                    // Find the pairs of characters
                    String[] PairsInWord = LetterPairs(Words[w]);

                    for (int p = 0; p < PairsInWord.Length; p++)
                    {
                        AllPairs.Add(PairsInWord[p]);
                    }
                }
            }

            return AllPairs;
        }

        /// <summary>
        /// Generates an array containing every 
        /// two consecutive letters in the input string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string[] LetterPairs(string str)
        {
            int numPairs = str.Length - 1;

            string[] pairs = new string[numPairs];

            for (int i = 0; i < numPairs; i++)
            {
                pairs[i] = str.Substring(i, 2);
            }

            return pairs;
        }

        #endregion


    }
}
