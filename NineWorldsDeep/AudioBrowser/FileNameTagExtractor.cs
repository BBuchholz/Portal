using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NineWorldsDeep.AudioBrowser
{
    public class FileNameTagExtractor
    {
        public List<string> ExtractFromString(string s)
        {
            //to account for matching multiple patterns, we return a list
            List<string> tags = new List<string>();

            Regex r = new Regex("([A-Za-z]+)");
            foreach (Match m in r.Matches(s))
            {
                tags.Add(m.Value.ToLower());
            }

            return tags;
        }
    }
}