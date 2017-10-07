using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hive
{
    public abstract class HiveSpore
    {
        public string Name { get; protected set; }
        public HiveLobe HiveLobe { get; private set; }

        public HiveSpore(string name, HiveLobe parentLobe)
        {
            Name = name;
            HiveLobe = parentLobe;
        }

        protected HiveSpore(HiveLobe parentLobe)
        {
            HiveLobe = parentLobe;
        }

        public abstract HiveSporeType HiveSporeType { get; protected set; }
    }

    public enum HiveSporeType
    {
        Audio,
        Image,
        Pdf,
        Unknown,
        Xml
    }
}
