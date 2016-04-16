using System.Collections.Generic;
using System.Linq;

namespace NineWorldsDeep.FragmentCloud
{
    public class Converter
    {
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