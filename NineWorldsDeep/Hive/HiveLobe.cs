using System;
using NineWorldsDeep.Mnemosyne.V5;
using System.Collections.Generic;

namespace NineWorldsDeep.Hive
{
    public abstract class HiveLobe
    {
        public string Name { private set; get; }
        protected List<HiveSpore> SporesInternal { private set; get; }
        public IEnumerable<HiveSpore> Spores { get { return SporesInternal; } }

        public HiveLobe(string name)
        {
            this.Name = name;
            this.SporesInternal = new List<HiveSpore>();
        }
        
        public virtual void Add(HiveSpore spore)
        {
            //TODO: validate, check for duplicates, &c.
            SporesInternal.Add(spore);
        }
    }
}