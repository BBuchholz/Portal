using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Parser
{
    public class NwdUri
    {
        private Stack<string> invertedKeyStack;
        private string uri;
        private string currentTraversalUri;

        public NwdUri(string uri)
        {
            this.uri = uri;
            ResetStack();
        }

        public string URI
        {
            get { return uri; }
        }

        public string CurrentTraversalUri
        {
            get { return currentTraversalUri; }
        }

        public void ResetStack()
        {
            invertedKeyStack = new Stack<string>();
            currentTraversalUri = "";

            string[] keys = uri.Split('/');

            for (int i = keys.Length - 1; i >= 0; i--)
            {
                invertedKeyStack.Push(keys[i]);
            }
        }

        public string PopNodeName()
        {
            if (!HasNodeKeysInStack())
            {
                return null;
            }

            string nodeName = invertedKeyStack.Pop();

            if(currentTraversalUri.Length > 0 &&
                !currentTraversalUri.EndsWith("/"))
            {
                currentTraversalUri += "/";
            }

            currentTraversalUri += nodeName;

            return nodeName;
        }

        public bool HasNodeKeysInStack()
        {
            return invertedKeyStack.Count > 0;
        }

        public static bool MatchNodeName(string nodeName, string nameToMatch)
        {
            if (nodeName.EndsWith("[*]"))
            {
                //match using "StartsWith"
                string startPattern = nodeName.Substring(0, nodeName.IndexOf("[*]"));
                return nameToMatch.StartsWith(startPattern);
            }
            else
            {
                //look for exact match
                return nameToMatch.Equals(nodeName);
            }
        }
    }
}
