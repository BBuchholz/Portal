using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NineWorldsDeep.Warehouse
{
    public class Tags
    {
        /// <summary>
        /// used FromHash() to retrieve from database
        /// and stores in SyncRootConfig keyed to hash
        /// </summary>
        /// <param name="sp"></param>
        public static void ExportTagsForProfile(SyncProfile sp, string sha1Hash)
        {
            //load if exists, will just be empty otherwise
            Dictionary<string, string> hashToTags = LoadFromHashToTagsIndex(sp);

            hashToTags[sha1Hash] = FromHash(sp, SyncDirection.Export, sha1Hash);

            List<string> lst = hashToTags.Select(x => "sha1Hash={" + x.Key + "} tags={" + x.Value + "}").ToList();

            File.WriteAllLines(HashToTagsIndexPath(sp), lst);
        }

        public static string HashToTagsIndexPath(SyncProfile sp)
        {
            return Configuration.SyncRootConfigFile(sp.Name, "HashToTagsIndex");
        }

        public static Dictionary<string, string> LoadFromHashToTagsIndex(SyncProfile sp)
        {
            Dictionary<string, string> hashToTagString = new Dictionary<string, string>();
            Parser.Parser p = new Parser.Parser();

            //use syncprofile to load an existing one if it is there
            string exportedTagsIndexPath = HashToTagsIndexPath(sp);

            if (File.Exists(exportedTagsIndexPath))
            {
                foreach (string lineItem in File.ReadLines(exportedTagsIndexPath))
                {
                    string hash = p.Extract("sha1Hash", lineItem);
                    string tagString = p.Extract("tags", lineItem);

                    hashToTagString[hash] = tagString;
                }
            }

            return hashToTagString;
        }

        public static string FromHash(SyncProfile sp,
                                      SyncDirection direction,
                                      string sha1Hash)
        {
            string tags = "";

            switch (direction)
            {
                case SyncDirection.Import:

                    //load by using profile to get sync root
                    //which gives us NWD/config
                    string fileHashIndexPath = Configuration.SyncRootConfigFile(sp.Name, "FileHashIndex");
                    string tagIndexPath = Configuration.SyncRootConfigFile(sp.Name, "TagIndex");

                    if (File.Exists(fileHashIndexPath) && File.Exists(tagIndexPath))
                    {
                        Parser.Parser p = new Parser.Parser();
                        List<string> paths = new List<string>();

                        //get all paths matching hash (may be multiple files, if copied in multiple places)
                        foreach (string lineItem in File.ReadLines(fileHashIndexPath))
                        {
                            string path = p.Extract("path", lineItem);
                            string hash = p.Extract("sha1Hash", lineItem);

                            if (hash.Equals(sha1Hash, StringComparison.CurrentCultureIgnoreCase))
                            {
                                paths.Add(path);
                            }
                        }

                        List<string> tagStrings = new List<string>();

                        //since we may have multiple files for a given hash, need to get all tags for those paths
                        foreach (string lineItem in File.ReadLines(tagIndexPath))
                        {
                            string path = p.Extract("path", lineItem);
                            string tagString = p.Extract("tags", lineItem);

                            if (paths.Contains(path))
                            {
                                tagStrings.Add(tagString);
                            }
                        }

                        //remove any duplicates
                        HashSet<string> uniqueTags = new HashSet<string>();

                        foreach (string tagString in tagStrings)
                        {
                            var theseTags = StringToList(tagString);

                            foreach (string tag in theseTags)
                            {
                                if (!uniqueTags.Contains(tag))
                                {
                                    uniqueTags.Add(tag);
                                }
                            }
                        }

                        tags = string.Join(", ", uniqueTags);
                    }

                    break;

                case SyncDirection.Export:

                    //get any tags from database tied to the
                    //supplied sha1Hash
                    SqliteDbAdapter db = new SqliteDbAdapter();
                    tags = db.GetTagsForSHA1Hash(sha1Hash);

                    break;
            }

            return tags;
        }

        public static List<string> StringToList(string tagString)
        {
            return tagString.Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}