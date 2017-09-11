using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hive.Spores
{
    public class HiveSporeFilePath : HiveSpore
    {
        public string Path { get; private set; }

        public HiveSporeFilePath(string filePath)
        {
            Name = System.IO.Path.GetFileName(filePath);
            Path = filePath;            
        }
    }
}
