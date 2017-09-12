using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hive.Lobes
{
    public class HiveLobeAudio : HiveLobe
    {
        public HiveLobeAudio(HiveRoot hr) : base("audio", hr)
        {
        }

        public override void Collect()
        {
            string hiveRootAudioSubFolderPath =
                ConfigHive.HiveRootAudioFolderPath(HiveRoot);

            foreach (string filePath in
                Directory.EnumerateFiles(hiveRootAudioSubFolderPath,
                                         "*.wav", SearchOption.AllDirectories))
            {
                Add(new Spores.HiveSporeFilePath(filePath));
            }
        }
    }
}
