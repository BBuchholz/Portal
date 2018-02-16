using NineWorldsDeep.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    public class TaggingMatrix
    {
        private MultiMap<string, string> tagsToPaths =
            new MultiMap<string, string>(StringComparer.CurrentCultureIgnoreCase);
        private MultiMap<string, string> pathsToTags =
            new MultiMap<string, string>(StringComparer.CurrentCultureIgnoreCase);

        public static readonly string TAG_UNTAGGED = "[[[UNTAGGED]]]";
        public static readonly string TAG_ALL = "[[[ALL]]]";

        public List<string> Tags { get { return tagsToPaths.Keys.ToList(); } }
        
        public TaggingMatrix()
        {
        }

        public List<string> PathsForTag(string tag)
        {
            return tagsToPaths[tag];
        }

        public TagCountDisplayItem GenerateAllPathsDisplayItem()
        {
            int count = GetPathsForTag(TAG_ALL).Count;

            return new TagCountDisplayItem()
            {
                Tag = TAG_ALL,
                Count = count
            };
        }

        public TagCountDisplayItem GenerateUntaggedPathsDisplayItem()
        {
            int count = GetPathsForTag(TAG_UNTAGGED).Count;

            return new TagCountDisplayItem()
            {
                Tag = TAG_UNTAGGED,
                Count = count
            };
        }

        /// <summary>
        /// passing in a null or whitespace tag parameter will get all paths
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public List<string> GetPathsForTag(string tag)
        {
            List<string> pathsForTag;

            if (string.IsNullOrWhiteSpace(tag))
            {
                tag = TAG_ALL;
            }

            if (tag != null && tagsToPaths.Keys.Contains(tag))
            {
                pathsForTag = tagsToPaths[tag];
            }
            else if (tag != null && tag.Equals(TAG_ALL))
            {
                pathsForTag = pathsToTags.Keys.ToList();
            }
            else if (tag != null && tag.Equals(TAG_UNTAGGED))
            {
                pathsForTag = GetUntaggedFilePaths();
            }
            else
            {
                pathsForTag = new List<string>();
            }

            return pathsForTag;
        }

        private List<string> GetUntaggedFilePaths()
        {
            List<string> untaggedPaths = new List<string>();

            foreach (string path in pathsToTags.Keys)
            {
                if(pathsToTags[path].Count < 1)
                {
                    untaggedPaths.Add(path);
                }
            }

            return untaggedPaths;
        }

        public void AddFolderAndAllSubfolders(string folderPath)
        {
            IEnumerable<string> files = 
                Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            foreach (string path in files)
            {
                pathsToTags.Add(path, null);
            }

        }

        public void LinkTagToPath(string tag, string path, bool includeNonLocalFiles = false)
        {
            if (includeNonLocalFiles || File.Exists(path))
            {
                tagsToPaths[tag].Add(path);
                pathsToTags[path].Add(tag);
            }
        }
    }
}
