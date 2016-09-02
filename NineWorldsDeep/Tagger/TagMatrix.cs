using NineWorldsDeep.Core;
using NineWorldsDeep.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NineWorldsDeep.Tagger
{
    public class TagMatrix
    {
        private MultiMap<string, string> tagFilesMap =
            new MultiMap<string, string>(StringComparer.CurrentCultureIgnoreCase);
        private MultiMap<string, string> fileTagsMap =
            new MultiMap<string, string>(StringComparer.CurrentCultureIgnoreCase);

        private List<string> folderPaths = new List<string>();

        private IFolderLoadStrategy folderLoadStrategy;
        public static readonly string TAG_UNTAGGED = "[[[UNTAGGED]]]";

        public void Clear()
        {
            tagFilesMap.ClearAll();
            fileTagsMap.ClearAll();
            folderPaths.Clear();
        }

        public void AddFolder(string folderPath)
        {
            folderPaths.Add(folderPath);

            IEnumerable<string> files;

            if (folderLoadStrategy != null)
            {
                files = folderLoadStrategy.GetFilesForFolder(folderPath);
            }
            else
            {
                //default behavior, get all
                files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            }

            foreach (string filePath in files)
            {
                fileTagsMap.Add(filePath, null);
            }

        }

        public void Add(List<FileElement> lst)
        {
            foreach (FileElement fe in lst)
            {
                fileTagsMap.Add(fe.Path, null);
            }
        }

        public bool FileElementHasMetaTag(FileElement fe, string metaTagName)
        {
            return !string.IsNullOrWhiteSpace(GetMetaTagForFileElement(fe, metaTagName));
        }

        public string GetMetaTagForFileElement(FileElement fe, string metaTagName)
        {
            string metaTag = null;

            //go through all tags for file element, return first for name, or return null if not found
            foreach (string tag in fileTagsMap[fe.Path])
            {
                if (TagString.IsMetaTag(tag) &&
                    metaTagName.Equals(TagString.ExtractTagNameFromMetaTag(tag)))
                {
                    metaTag = tag;
                }
            }

            return metaTag;
        }

        public IEnumerable<string> GetFilesForTag(string tag, FilePathFilter fpf)
        {
            return fpf.Filter(GetFilesForTag(tag));
        }

        public IEnumerable<string> GetFilesForTag(string tag)
        {
            IEnumerable<string> fileElementPaths;

            if (string.IsNullOrWhiteSpace(tag))
            {
                tag = "[[[ALL]]]";
            }

            if (tag != null && tagFilesMap.Keys.Contains(tag))
            {
                fileElementPaths = tagFilesMap[tag];
            }
            else if (tag != null && tag.Equals("[[[ALL]]]"))
            {
                fileElementPaths = fileTagsMap.Keys;
            }
            else if (tag != null && tag.Equals(TAG_UNTAGGED))
            {
                fileElementPaths = GetUntaggedFilePaths();
            }
            else
            {
                fileElementPaths = new List<string>();
            }

            return fileElementPaths;
        }

        public void PruneTagList()
        {
            List<string> toRemove = new List<string>();

            foreach (string tag in tagFilesMap.Keys)
            {
                if (tagFilesMap[tag].Count == 0)
                {
                    toRemove.Add(tag);
                }
            }

            foreach (string tag in toRemove)
            {
                tagFilesMap.RemoveKey(tag);
            }

            //PopulateTagListView();
        }

        private IEnumerable<string> GetUntaggedFilePaths()
        {
            List<string> lst = new List<string>();

            foreach (string fPath in fileTagsMap.Keys)
            {
                if (TagString.CountIgnoringTimeStampTags(fileTagsMap[fPath]) < 1)
                {
                    lst.Add(fPath);
                }
            }

            return lst;
        }

        public void UpdateTagString(FileElement fe, string tagString)
        {
            if (fe != null)
            {
                //remove all references to file element in tag map
                tagFilesMap.PurgeValue(fe.Path);

                //clear current tags in file map
                fileTagsMap.Clear(fe.Path);

                //add all tags for file
                SetTags(fe.Path, tagString);

                //prune unused tags
                PruneTagList();

                //update file element
                fe.TagString = tagString;
            }
        }

        public void LoadFromXml(string loadFilePath)
        {
            //load tags from xml tagFile
            if (File.Exists(loadFilePath))
            {
                var xDoc = XDocument.Load(loadFilePath);

                var fileTags = xDoc.Descendants("fileTags")
                    .Select(x => new {
                        Path = x.Attribute("filePath").Value,
                        Tags = x.Value
                    }).ToList();

                foreach (var x in fileTags)
                {
                    SetTags(x.Path, x.Tags);
                }
            }
        }

        [Obsolete("use LoadFromXml(string)")]
        public void LoadFromFile(string loadFilePath)
        {
            //load tags from xml tagFile
            if (File.Exists(loadFilePath))
            {
                var xDoc = XDocument.Load(loadFilePath);

                var fileTags = xDoc.Descendants("fileTags")
                    .Select(x => new {
                        Path = x.Attribute("filePath").Value,
                        Tags = x.Value
                    }).ToList();

                foreach (var x in fileTags)
                {
                    SetTags(x.Path, x.Tags);
                }
            }
        }

        private void SetTags(string filePath, string tagString)
        {
            //ensure key for filePaths without tags
            fileTagsMap.Add(filePath, null);

            foreach (string tag in TagString.Parse(tagString))
            {
                Link(filePath, tag);
            }
        }

        private void Link(string filePath, string tag)
        {
            if (File.Exists(filePath))
            {
                tagFilesMap.Add(tag, filePath);
                fileTagsMap.Add(filePath, tag);
            }
        }

        public IEnumerable<string> GetFilePaths()
        {
            return fileTagsMap.Keys;
        }

        //public IEnumerable<string> GetTags()
        //{
        //    return GetTags("");
        //}

        public IEnumerable<TagModelItem> GetTagModelItems(string filter)
        {
            List<TagModelItem> items = new List<TagModelItem>();

            List<string> tags = tagFilesMap.Keys.ToList<string>();
            tags = tags.Where(tag => tag.ToLower().Contains(filter.ToLower())).ToList();

            tags.Sort();

            foreach (string tag in tags)
            {
                TagModelItem tmi = new TagModelItem(tag);
                foreach(string path in tagFilesMap[tag])
                {
                    tmi.Link(new Sqlite.Model.FileModelItem(
                        Configuration.GetLocalDeviceDescription(), path));
                }

                items.Add(tmi);
            }

            items.Insert(0, new TagModelItem("[[[ALL]]]"));
            items.Insert(1, new TagModelItem("[[[UNTAGGED]]]"));

            return items;
        }

        [Obsolete("use GetTagModelItems()")]
        public IEnumerable<string> GetTags(string filter)
        {
            List<string> tags = tagFilesMap.Keys.ToList<string>();

            tags = tags.Where(tag => tag.ToLower().Contains(filter.ToLower())).ToList();

            tags.Sort();

            tags.Insert(0, "[[[ALL]]]");
            tags.Insert(1, "[[[UNTAGGED]]]");

            return tags;
        }
        
        //public string GetTagString(FileElement fe)
        //{
        //    string tagString = "";

        //    if (fe != null)
        //    {
        //        tagString = TagString.Parse(fileTagsMap[fe.Path]);
        //    }

        //    return tagString;
        //}

        public string GetTagString(string filePath)
        {
            string tagString = "";

            if (!string.IsNullOrWhiteSpace(filePath) &&
                fileTagsMap.ContainsKey(filePath))
            {
                tagString = TagString.Parse(fileTagsMap[filePath]);
            }

            return tagString;
        }

        [Obsolete("use SaveToXml(string)")]
        public void Save(string saveFilePath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement filesEl = doc.CreateElement("files");

            foreach (string filePath in GetFilePaths())
            {
                XmlElement fileTagsEl = doc.CreateElement("fileTags");
                fileTagsEl.SetAttribute("filePath", filePath);
                fileTagsEl.InnerText = GetTagString(filePath);
                filesEl.AppendChild(fileTagsEl);
            }

            doc.AppendChild(filesEl);
            doc.Save(saveFilePath);
        }

        public void SaveToXml(string saveFilePath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement filesEl = doc.CreateElement("files");

            foreach (string filePath in GetFilePaths())
            {
                XmlElement fileTagsEl = doc.CreateElement("fileTags");
                fileTagsEl.SetAttribute("filePath", filePath);
                fileTagsEl.InnerText = GetTagString(filePath);
                filesEl.AppendChild(fileTagsEl);
            }

            doc.AppendChild(filesEl);
            doc.Save(saveFilePath);
        }

        public void SetFolderLoadStrategy(IFolderLoadStrategy fls)
        {
            this.folderLoadStrategy = fls;
        }

        /// <summary>
        /// will retrieve load all paths and tags for those paths
        /// where the path begins with filePathTopFolder
        /// 
        /// any level of hierarchy is supported (i.e. "C:\NWD-AUX\" 
        /// will work, as will "C:\NWD-AUX\voicememos", with
        /// the first one including the results from the second
        /// one, thus respecting hierarchy)
        /// 
        /// Also note, passing "" as a parameter will match
        /// all paths for which tags have been stored
        /// </summary>
        /// <param name="filePathTopFolder"></param>
        public void LoadFromDb(string filePathTopFolder)
        {
            NineWorldsDeep.Db.SqliteDbAdapter db =
                new NineWorldsDeep.Db.SqliteDbAdapter();

            //retrieve all path to tag mappings
            List<NineWorldsDeep.Db.PathTagLink> lst =
                db.GetPathTagLinks(filePathTopFolder);

            //foreach Link(path, tag)
            foreach (NineWorldsDeep.Db.PathTagLink ptl in lst)
            {
                Link(ptl.PathValue, ptl.TagValue);
            }
        }

        public void SaveToDb()
        {
            NineWorldsDeep.Db.SqliteDbAdapter db =
                new NineWorldsDeep.Db.SqliteDbAdapter();

            //get unique list of paths (as dictionary, id = -1)
            Dictionary<string, int> pathsToIds =
                new Dictionary<string, int>();

            foreach (string path in fileTagsMap.Keys)
            {
                pathsToIds[path] = -1;
            }

            //get unique list of tags (as dictionary, id = -1)
            Dictionary<string, int> tagsToIds =
                new Dictionary<string, int>();

            foreach (string tag in tagFilesMap.Keys)
            {
                //store as lower case, was throwing errors below
                //TODO: encapsulate tag in class and make comparison case-insensitive                
                tagsToIds[tag.ToLower()] = -1;
            }

            //store paths
            db.StorePaths(pathsToIds.Keys.ToList<string>());

            //store tags
            db.StoreTags(tagsToIds.Keys.ToList<string>());

            //store device in device table
            db.StoreDevice("Main Laptop", "Main Laptop", "hp 2000", "laptop");

            //get device id
            int deviceId = db.GetDeviceId("Main Laptop", "Main Laptop", "hp 2000", "laptop");

            //populate path ids in dict from above
            db.PopulatePathIds(pathsToIds);

            //populate tag ids in dict from above
            db.PopulateTagIds(tagsToIds);

            //iterate and create mappings
            List<NineWorldsDeep.Db.PathToTagMapping> mappings =
                new List<NineWorldsDeep.Db.PathToTagMapping>();

            foreach (string path in fileTagsMap.Keys)
            {
                int pathId = pathsToIds[path];

                var tags = fileTagsMap[path];

                foreach (string tag in tags)
                {
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        int tagId = tagsToIds[tag.ToLower()];

                        mappings.Add(
                            new NineWorldsDeep.Db.PathToTagMapping()
                            {
                                PathId = pathId,
                                TagId = tagId,
                                DeviceId = deviceId
                            });
                    }
                }
            }

            //store mappings
            db.StorePathToTagMappings(mappings);

        }

        public void RemovePath(string path)
        {
            tagFilesMap.PurgeValue(path);
            fileTagsMap.RemoveKey(path);
        }
    }
}