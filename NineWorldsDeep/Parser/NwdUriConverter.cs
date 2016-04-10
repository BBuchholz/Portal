using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Parser
{
    public class NwdUriConverter
    {
        /// <summary>
        /// for a list of Android paths (such as those from 
        /// FileHashIndex or TagFileIndex in Gauntlet), finds all of 
        /// the roots from all entries containing "NWD".
        /// the first instance of "NWD" is where the break
        /// occurs, so "/storage/0/NWD-SNDBX/test1/NWD/..."
        /// would store "/storage/0" not 
        /// "/storage/0/NWD-SNDBX/test1/..."
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static List<string> GetAndroidPathRoots(IEnumerable<string> paths)
        {
            //obviously, from the name, intended for Android Path handling

            //TODO: paths refactor, add to NWD.NwdUriSomething
            //TO GET ALL ROOTS (may not need this here, this random thought was aiming for path based, not hash based. If it isn't needed, add these comments as comments for a Utility method somewhere, with a TODO task to implement included :)
            //go through paths of both files,
            //any path that contains "NWD",
            //find the first instance,
            //and store the preceding part as one
            //root path. This should find external and internal.
            //then, go back through list, trimming
            //all instances all root paths
            //and get those paths that do not contain "NWD"
            //(just by trimming, I'm not referring to a Contains() test)
            //this will get folders like Pictures/Screenshots and DCIM/camera

            throw new NotImplementedException();
        }

        public static string NwdUriToFileSystemPath(string uri)
        {
            Stack<string> nodeStack = UriToNodeNameStack(uri);

            string path = "";

            while (nodeStack.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    path = nodeStack.Pop() + @"\" + path;
                }
                else
                {
                    path = nodeStack.Pop();
                }
            }

            path = @"C:\" + path;

            return path;

        }

        /// <summary>
        /// cleans extra '/' characters from a uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string SanitizeUri(string uri)
        {
            while (uri.Contains(@"//"))
            {
                uri = uri.Replace(@"//", @"/");
            }

            return uri;
        }

        /// <summary>
        /// trims the path portion of the uri and returns just the
        /// endpoint node name. eg uri "NWD/path/to/resource" would
        /// return "resource"
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string NwdUriNodeName(string uri)
        {
            return uri.Split('/').Last();
        }

        /// <summary>
        /// removes endpoint node from given uri, returning trimmed
        /// uri. eg uri "NWD/path/to/resource" would return
        /// "NWD/path/to"
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string TrimNwdUriNodeName(string uri)
        {
            Stack<string> nodeStack = UriToNodeNameStack(uri);

            if (nodeStack.Count > 0)
                nodeStack.Pop(); //remove last

            string trimmed = "";

            while (nodeStack.Count > 0)
            {
                trimmed = nodeStack.Pop() + '/' + trimmed;
            }

            return trimmed;
        }

        private static Stack<string> UriToNodeNameStack(string uri)
        {
            Stack<string> nodeStack = new Stack<string>();

            foreach (string node in uri.Split('/'))
            {
                if (!string.IsNullOrWhiteSpace(node))
                {
                    nodeStack.Push(node);
                }
            }

            return nodeStack;
        }
    }
}
