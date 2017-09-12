using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hive.Lobes
{
    public class HiveLobeImages : HiveLobe
    {
        public HiveLobeImages(HiveRoot hr) : base("images", hr)
        {
        }

        public override void Collect()
        {
            string hiveRootImagesSubFolderPath =
                ConfigHive.HiveRootImagesFolderPath(HiveRoot);

            foreach(string filePath in
                Directory.EnumerateFiles(hiveRootImagesSubFolderPath,
                                         "*.*", SearchOption.AllDirectories)
                                        .Where(s => s.EndsWith(".bmp") ||
                                                    s.EndsWith(".gif") ||
                                                    s.EndsWith(".ico") ||
                                                    s.EndsWith(".jpg") ||
                                                    s.EndsWith(".png") ||
                                                    s.EndsWith(".tiff")))
            {
                Add(new Spores.HiveSporeFilePath(filePath));
            }
        }
    }
}
