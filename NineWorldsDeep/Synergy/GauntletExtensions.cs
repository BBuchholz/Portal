using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Synergy
{
    public static class GauntletExtensions
    {
        private static Parser.Parser p = new Parser.Parser();

        public static string ExtractLastByKey(this string s, string extractKey)
        {
            return p.ExtractLastOne(extractKey, s);
        }

        public static string ExtractByKey(this string s, string extractKey)
        {
            string openTag = extractKey + "={";

            if (s.Contains(openTag))
            {
                int startIdx =
                        s.IndexOf(openTag) + openTag.Length;

                int endIdx = s.IndexOf("}", startIdx);

                return s.Substring(startIdx, endIdx - startIdx);
            }

            return null;
        }

        public static string[] ToStringArray(this IEnumerable<ToDoItem> ie)
        {
            List<string> lst = new List<string>();

            foreach (ToDoItem tdi in ie)
            {
                lst.Add(tdi.Description);
            }

            return lst.ToArray();
        }

        public static string[] ToFragmentArray(this IEnumerable<ToDoItem> ie)
        {
            List<string> lst = new List<string>();

            foreach (ToDoItem tdi in ie)
            {
                lst.Add(tdi.FragmentString);
            }

            return lst.ToArray();
        }
    }
}
