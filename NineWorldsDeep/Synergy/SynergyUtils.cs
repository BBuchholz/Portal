using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Synergy
{
    public class SynergyUtils
    {
        public static string ProcessListName(string text)
        {
            text = text.Trim();

            text = ReplaceInvalidChars(text);

            if(text.Contains(" ") ||
                IsAllLower(text))
            {
                //respect dashes
                text = text.Replace("-", " - ");

                //remove double spaces
                while(text.Contains("  "))
                {
                    text = text.Replace("  ", " ");
                }
            }

            text = ToPascalCase(text).Replace(" ", "");

            return text;
        }

        private static string ToPascalCase(string text)
        {
            TextInfo ti = new CultureInfo("en-US", false).TextInfo;
            return ti.ToTitleCase(text);
        }

        private static string ReplaceInvalidChars(string text)
        {
            StringBuilder sb = new StringBuilder();

            foreach(char c in text)
            {
                if (char.IsLetterOrDigit(c) ||
                    char.IsWhiteSpace(c) ||
                    c.Equals('-'))
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append("_");
                }
            }

            return sb.ToString();
        }

        public static bool IsAllLower(string text)
        {
            bool isLower = true;

            foreach(char c in text)
            {
                if (!char.IsLower(c))
                {
                    isLower = false;
                }
            }

            return isLower;
        }
    }        
}
