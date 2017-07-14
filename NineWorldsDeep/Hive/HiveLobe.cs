using System;
using NineWorldsDeep.Mnemosyne.V5;
using System.Collections.Generic;

namespace NineWorldsDeep.Hive
{
    public class HiveLobe
    {
        public string Name { private set; get; }
        public List<HiveSpore> Spores { private set; get; }
        
        public HiveLobe(string name)
        {
            this.Name = name;
            this.Spores = new List<HiveSpore>();
        }
        
        public void Add(HiveSpore spore)
        {
            //TODO: validate, check for duplicates, &c.
            Spores.Add(spore);
        }
    }
}