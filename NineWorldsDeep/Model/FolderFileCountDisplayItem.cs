using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Model
{
    public class FolderFileCountDisplayItem
    {
        public string FolderName { get; set; }
        public int FileCount { get; set; }

        public override string ToString()
        {
            var output = FolderName;

            if(FileCount >= 0)
            {
                output += " (" + FileCount + " files found)";
            }

            return output;
        }
    }
}
