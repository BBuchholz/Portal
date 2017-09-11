using System;
using System.Collections.Generic;
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
            UI.Display.Message("Collect() method not implemented for HiveLobe: " + this.Name); //use HiveRoot and Configuration class to get files from folder hierarchy and file type
        }
    }
}
