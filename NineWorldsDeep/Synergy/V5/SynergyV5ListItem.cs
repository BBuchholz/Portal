using System;

namespace NineWorldsDeep.Synergy.V5
{
    public class SynergyV5ListItem
    {
        public SynergyV5ListItem(string itemValue)
        {
            ItemValue = itemValue;
        }

        public SynergyV5ListItem(string itemValue, 
                                 DateTime itemActivatedAtTime, 
                                 DateTime completedAtTime, 
                                 DateTime archivedAtTime) : this(itemValue)
        {
            this.ActivatedAt = itemActivatedAtTime;
            this.CompletedAt = completedAtTime;
            this.ArchivedAt = archivedAtTime;
        }

        //should this become a SynergyItem object? to mirror db?
        public string ItemValue { get; private set; }

        public DateTime ActivatedAt { get; private set; }
        public DateTime CompletedAt { get; private set; }
        public DateTime ArchivedAt { get; private set; }
    }
}