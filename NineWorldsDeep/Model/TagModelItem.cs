using NineWorldsDeep.Sqlite.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Model
{
    public class TagModelItem
    {
        private string mTag;
        private List<FileModelItem> mFiles =
            new List<FileModelItem>();

        public TagModelItem(string tag)
        {
            mTag = tag;
        }

        public void Link(FileModelItem fmi)
        {
            if(fmi != null && !mFiles.Contains(fmi))
            {
                mFiles.Add(fmi);
            }
        }

        public IEnumerable<FileModelItem> Files
        {
            get { return mFiles; }
        }

        public string Tag
        {
            get { return mTag; }
        }

        public override string ToString()
        {
            string output = mTag;

            if (mFiles.Count > 0)
            {
                output += " (" + mFiles.Count + ")";
            }

            return output;
        }
    }
}
