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
        public List<HiveLobe> Lobes { private set; get; }
        public string Name { get; internal set; }

        public HiveRoot(string name)
        {
            Name = name;
            Lobes = new List<HiveLobe>();
        }

        public void Add(HiveLobe lobe)
        {
            //TODO: configure/validate/&c.
            Lobes.Add(lobe);
        }
    }
}
