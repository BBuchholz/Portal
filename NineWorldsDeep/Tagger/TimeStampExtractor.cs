using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace NineWorldsDeep.Tagger
{
    public class TimeStampExtractor
    {
        private string outputFormat = "yyyyMMddHHmmss";
        private Dictionary<string, string> regexDateTimeMap =
            new Dictionary<string, string>();

        public TimeStampExtractor()
        {
            loadRegexToDateTimeMapStrings();
        }

        private void loadRegexToDateTimeMapStrings()
        {
            //patterns from the input data
            regexDateTimeMap.Add(@"\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}", "yyyy-MM-dd_HH-mm-ss");
            regexDateTimeMap.Add(@"\d{10}", "MMddyyHHmm");
            regexDateTimeMap.Add(@"\d{12}", "yyyyMMddHHmm");
            regexDateTimeMap.Add(@"\d{8}", "yyyyMMdd");
        }

        public List<string> ExtractFromString(string s)
        {
            //to account for matching multiple patterns, we return a list
            List<string> extractedPatternsInNormalizedForm = new List<string>();

            foreach (KeyValuePair<string, string> kv in regexDateTimeMap)
            {
                Regex r = new Regex(kv.Key);
                Match m = r.Match(s);
                if (m.Success)
                {
                    try
                    {
                        DateTime dt = DateTime.ParseExact(m.Value, kv.Value, CultureInfo.InvariantCulture);
                        extractedPatternsInNormalizedForm.Add(dt.ToString(outputFormat));
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }
            }

            return extractedPatternsInNormalizedForm;
        }

        public string ExtractFromFileAttributes(string path)
        {
            string extracted = null;

            DateTime dt = File.GetCreationTime(path);
            extracted = dt.ToString(outputFormat);

            return extracted;
        }
    }
}