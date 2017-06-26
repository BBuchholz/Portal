using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hive
{
    public class HiveRoot
    {
        public MultiMap<string, HiveFileGrouping> Files { private set; get; }
        public string Name { get; internal set; }

        public HiveRoot()
        {
            Files = new MultiMap<string, HiveFileGrouping>();

            //add more here as we support more types
            AddGrouping("xml");
            AddGrouping("images");
            AddGrouping("audio");
            AddGrouping("pdfs");
        }

        private void AddGrouping(string groupingName)
        {
            var hiveFileGrouping = new HiveFileGrouping(groupingName);
            Files.Add(hiveFileGrouping.Name, hiveFileGrouping);
        }
    }
}
