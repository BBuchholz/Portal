using NineWorldsDeep.Core;
using NineWorldsDeep.Sqlite.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace NineWorldsDeep.Warehouse
{
    public class Tags
    {
        private static StringDictionary hashToTagString =
            new StringDictionary();

        /// <summary>
        /// used FromHash() to retrieve from database
        /// and stores in SyncRootConfig keyed to hash
        /// </summary>
        /// <param name="sp"></param>
        public static void ExportTagsForProfile(SyncProfile sp, string sha1Hash)
        {
            ////load if exists, will just be empty otherwise
            //Dictionary<string, string> hashToTags = LoadFromHashToTagsIndex(sp);

            //hashToTags[sha1Hash] = FromHash(sp, SyncDirection.Export, sha1Hash);

            //List<string> lst = hashToTags.Select(x => "sha1Hash={" + x.Key + "} tags={" + x.Value + "}").ToList();

            //File.WriteAllLines(HashToTagsIndexPath(sp), lst);

            //for now, supporting both formats (though one gets 
            //lost on sync depending on the sequence, which is 
            //why this new way is being phased in)
            ExportTagsForProfileToPath(sp, sha1Hash, true);
            ExportTagsForProfileToPath(sp, sha1Hash, false);
        }

        public static void ExportTagsForProfileToXml(string deviceName, SyncProfile sp, List<FileModelItem> files)
        {
            Xml.XmlExporter xe = new Xml.XmlExporter();
            string targetFile = Configuration.SyncRootNewXmlExportFile(sp);

            xe.Export(deviceName, null, files, targetFile); 
        }

        public static void ExportTagsForProfileToPath(SyncProfile sp, string sha1Hash, bool timeStampList)
        {
            //load if exists, will just be empty otherwise
            Dictionary<string, string> hashToTags = LoadFromHashToTagsIndex(sp);

            hashToTags[sha1Hash] = GetTagStringForHash(sha1Hash);

            List<string> lst = hashToTags.Select(x => "sha1Hash={" + x.Key + "} tags={" + x.Value + "}").ToList();

            File.WriteAllLines(HashToTagsIndexPath(sp, timeStampList), lst);
        }

        public static string HashToTagsIndexPath(SyncProfile sp, bool timeStampList)
        {
            string name = "HashToTagsIndex";

            if (timeStampList)
            {
                name = NwdUtils.GetTimeStamp_yyyyMMddHHmmss() + "-" + name;
            }

            return Configuration.SyncRootConfigFile(sp.Name, name);
        }

        public static Dictionary<string, string> LoadFromHashToTagsIndex(SyncProfile sp)
        {
            Dictionary<string, string> hashToTagString = new Dictionary<string, string>();
            Parser.Parser p = new Parser.Parser();

            //use syncprofile to load an existing one if it is there
            string exportedTagsIndexPath = HashToTagsIndexPath(sp, false);

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

        //[Obsolete("use ImportForHash and ExportForHash")]
        //public static string FromHash(SyncProfile sp,
        //                              SyncDirection direction,
        //                              string sha1Hash,
        //                              bool tagsFromXmlNotKeyVal)
        //{
        //    string tags = "";

        //    switch (direction)
        //    {
        //        case SyncDirection.Import:

        //            ////load by using profile to get sync root
        //            ////which gives us NWD/config
        //            //string fileHashIndexPath = Configuration.SyncRootConfigFile(sp.Name, "FileHashIndex");
        //            //string tagIndexPath = Configuration.SyncRootConfigFile(sp.Name, "TagIndex");

        //            //if (File.Exists(fileHashIndexPath) && File.Exists(tagIndexPath))
        //            //{
        //            //    Parser.Parser p = new Parser.Parser();
        //            //    List<string> paths = new List<string>();

        //            //    //get all paths matching hash (may be multiple files, if copied in multiple places)
        //            //    foreach (string lineItem in File.ReadLines(fileHashIndexPath))
        //            //    {
        //            //        string path = p.Extract("path", lineItem);
        //            //        string hash = p.Extract("sha1Hash", lineItem);

        //            //        if(!string.IsNullOrWhiteSpace(path) &&
        //            //            !string.IsNullOrWhiteSpace(hash))
        //            //        {
        //            //            if (hash.Equals(sha1Hash, StringComparison.CurrentCultureIgnoreCase))
        //            //            {
        //            //                paths.Add(path);
        //            //            }
        //            //        }
        //            //    }

        //            //    List<string> tagStrings = new List<string>();

        //            //    //since we may have multiple files for a given hash, need to get all tags for those paths
        //            //    foreach (string lineItem in File.ReadLines(tagIndexPath))
        //            //    {
        //            //        string path = p.Extract("path", lineItem);
        //            //        string tagString = p.Extract("tags", lineItem);

        //            //        if (paths.Contains(path))
        //            //        {
        //            //            tagStrings.Add(tagString);
        //            //        }
        //            //    }

        //            //    //remove any duplicates
        //            //    HashSet<string> uniqueTags = new HashSet<string>();

        //            //    foreach (string tagString in tagStrings)
        //            //    {
        //            //        var theseTags = StringToList(tagString);

        //            //        foreach (string tag in theseTags)
        //            //        {
        //            //            if (!uniqueTags.Contains(tag))
        //            //            {
        //            //                uniqueTags.Add(tag);
        //            //            }
        //            //        }
        //            //    }

        //            //    tags = string.Join(", ", uniqueTags);
        //            //}

        //            if (tagsFromXmlNotKeyVal)
        //            {
        //                tags = GetTagsFromXmlFile(sp, sha1Hash);
        //            }
        //            else
        //            {
        //                tags = GetTagsFromKeyValFile(sp, sha1Hash);
        //            }

        //            break;

        //        case SyncDirection.Export:

        //            //get any tags from database tied to the
        //            //supplied sha1Hash
        //            SqliteDbAdapter db = new SqliteDbAdapter();
        //            tags = db.GetTagsForSHA1Hash(sha1Hash);

        //            break;
        //    }

        //    return tags;
        //}

        public static string GetTagStringForHash(string sha1Hash)
        {
            //get any tags from database tied to the
            //supplied sha1Hash
            SqliteDbAdapter db = new SqliteDbAdapter();
            return db.GetTagsForSHA1Hash(sha1Hash);
        }

        public static string ImportForHash(SyncProfile sp,
                                           string sha1Hash,
                                           bool tagsFromXmlNotKeyVal)
        {
            String tagString = "";

            if (tagsFromXmlNotKeyVal)
            {
                tagString = GetTagsFromXmlFile(sp, sha1Hash);
            }
            else
            {
                tagString = GetTagsFromKeyValFile(sp, sha1Hash);
            }

            return tagString;
        }

        public static void ReloadFromXmlFile(SyncProfile sp)
        {
            //load from file
            string xmlPath =
                Configuration.SyncRootMostRecentXmlFile(sp.Name);

            if (!string.IsNullOrWhiteSpace(xmlPath))
            {
                Xml.XmlImporter xi = new Xml.XmlImporter(xmlPath);

                foreach (FileModelItem fmi in xi.GetFiles())
                {
                    String tagString = string.Join(", ", fmi.GetTags());

                    foreach (HashModelItem hash in fmi.GetHashes())
                    {
                        hashToTagString[hash.GetHash()] = tagString;
                    }
                }
            }
        }

        private static string GetTagsFromXmlFile(SyncProfile sp, string sha1Hash)
        {
            if (hashToTagString.Count < 1)
            {
                ReloadFromXmlFile(sp);
            }

            String tagString = "";
            
            if (hashToTagString.ContainsKey(sha1Hash))
            {
                tagString = hashToTagString[sha1Hash];
            }

            return tagString;
        }

        private static string GetTagsFromKeyValFile(SyncProfile sp, string sha1Hash)
        {
            //load by using profile to get sync root
            //which gives us NWD/config
            string fileHashIndexPath = Configuration.SyncRootConfigFile(sp.Name, "FileHashIndex");
            string tagIndexPath = Configuration.SyncRootConfigFile(sp.Name, "TagIndex");

            string tags = "";

            if (File.Exists(fileHashIndexPath) && File.Exists(tagIndexPath))
            {
                Parser.Parser p = new Parser.Parser();
                List<string> paths = new List<string>();

                //get all paths matching hash (may be multiple files, if copied in multiple places)
                foreach (string lineItem in File.ReadLines(fileHashIndexPath))
                {
                    string path = p.Extract("path", lineItem);
                    string hash = p.Extract("sha1Hash", lineItem);

                    if (!string.IsNullOrWhiteSpace(path) &&
                        !string.IsNullOrWhiteSpace(hash))
                    {
                        if (hash.Equals(sha1Hash, StringComparison.CurrentCultureIgnoreCase))
                        {
                            paths.Add(path);
                        }
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