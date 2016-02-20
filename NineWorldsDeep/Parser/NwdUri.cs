﻿using System;
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

        public NwdUri(string uri)
        {
            this.uri = uri;
            ResetStack();
        }

        public void ResetStack()
        {
            invertedKeyStack = new Stack<string>();

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

            return invertedKeyStack.Pop();
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
