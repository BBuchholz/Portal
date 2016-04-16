using System;
using System.Collections.Generic;

namespace NineWorldsDeep.Studio
{
    public class LyricBit
    {
        private int id;
        private int snippetLength = 20;
        private string mostRecentKey;
        private Dictionary<string, string> timeStampedVersions;

        public LyricBit(int id)
        {
            this.id = id;
            timeStampedVersions = new Dictionary<string, string>();
        }

        public int Id
        {
            get { return id; }
        }

        public string CurrentVersion
        {
            get
            {
                if (mostRecentKey != null)
                {
                    return timeStampedVersions[mostRecentKey];
                }
                else
                {
                    return "[empty]";
                }
            }
        }

        public string Snippet
        {
            get
            {
                if (CurrentVersion.Length > snippetLength)
                {
                    return CurrentVersion.Substring(0, snippetLength - 3) + "...";
                }
                else
                {
                    return CurrentVersion;
                }
            }
        }

        public override string ToString()
        {
            return Id + ":" + Snippet;
        }

        public void Update(string text)
        {
            mostRecentKey = DateTime.Now.ToString("yyyyMMddHHmmss");
            timeStampedVersions[mostRecentKey] = text;
        }
    }
}