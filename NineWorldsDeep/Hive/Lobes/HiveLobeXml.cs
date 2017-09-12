using NineWorldsDeep.Core;
using NineWorldsDeep.Hive.Spores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hive.Lobes
{
    public class HiveLobeXml : HiveLobe
    {
        public HiveLobeXml(HiveRoot hr) : base("xml", hr)
        {
        }

        public override void Collect()
        {
            string hiveRootXmlSubFolderPath = 
                ConfigHive.HiveRootXmlFolderPath(HiveRoot);

            foreach(string filePath in 
                Directory.GetFiles(hiveRootXmlSubFolderPath,
                                   "*.xml", SearchOption.AllDirectories))
            {
                Add(new HiveSporeFilePath(filePath));
            }
        }
    }
}
