using System;
using System.Collections.Generic;

namespace NineWorldsDeep.Tagger
{
    public class TagString
    {
        private static List<string> PurgeNullAndWhiteSpaceEntries(List<string> lst)
        {
            List<string> output = new List<string>();

            foreach (string tag in lst)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    output.Add(tag);
                }
            }

            return output;
        }

        public static string Parse(List<string> lst)
        {
            //I know this isn't the cleanest fix
            //but prevents null tags that were ending up
            //in our TagMatrix, could potentially 
            //refactor so TagMatrix handles this
            lst = PurgeNullAndWhiteSpaceEntries(lst);
            string tagString = String.Join(", ", lst);
            return tagString;
        }

        public static List<string> Parse(string tagString)
        {
            List<string> lst = new List<string>();
            foreach (string tag in tagString.Split(','))
            {
                if (!string.IsNullOrWhiteSpace(tag))
                    lst.Add(tag.Trim());
            }
            return lst;
        }


        public static int CountIgnoringTimeStampTags(List<string> tags)
        {
            int count = 0;

            foreach (string tag in tags)
            {
                if (TagString.IsMetaTag(tag) &&
                    TagString.ExtractTagNameFromMetaTag(tag)
                             .Equals("TimeStamp", 
                                     StringComparison.CurrentCultureIgnoreCase))
                {
                    //ignore
                }
                else
                {
                    //TODO: investigate and fix this
                    //still getting null tags, not sure where, but this is a quick fix
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        count += 1;
                    }
                }

            }

            return count;
        }


        public static bool IsMetaTag(string tag)
        {
            //nb: metatag format is "Tag Name: Tag Value"

            if (string.IsNullOrWhiteSpace(tag))
            {
                return false;
            }

            //must contain one, and ONLY one, colon
            if (tag.IndexOf(":") == -1 ||
                tag.IndexOf(":") != tag.LastIndexOf(":"))
            {
                return false;
            }

            return true;
        }

        public static string ExtractTagNameFromMetaTag(string tag)
        {
            //nb: metatag format is "Tag Name: Tag Value"

            string[] seps = new string[] { ":" };

            return tag.Split(seps, StringSplitOptions.None)[0].Trim();
        }
    }
}