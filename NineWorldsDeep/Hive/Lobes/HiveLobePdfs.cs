using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hive.Lobes
{
    public class HiveLobePdfs : HiveLobe
    {
        public HiveLobePdfs(HiveRoot hr) : base("pdfs", hr)
        {
        }

        public override void Collect()
        {
            string hiveRootPdfsSubFolderPath =
                ConfigHive.HiveRootPdfsFolderPath(HiveRoot);

            foreach(string filePath in
                System.IO.Directory.EnumerateFiles(
                    hiveRootPdfsSubFolderPath, 
                    "*.pdf",                                              
                    System.IO.SearchOption.AllDirectories))
            {
                Add(new Spores.HiveSporeFilePath(filePath, this));
            }
        }
    }
}
