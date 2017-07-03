using NineWorldsDeep.Sqlite.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Model
{
    public class TagCountDisplayItem
    {
        public string Tag { set; get; }
        public int Count { set; get; }
                      
        public override string ToString()
        {
            string output = Tag;

            if (Count > 0)
            {
                output += " (" + Count + ")";
            }

            return output;
        }
    }
}
