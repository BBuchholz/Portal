using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Synergy.V5
{
    public class SynergyV5List
    {
        public SynergyV5List(string listName, bool deferredLoad = false)
        {
            ListName = listName;
            ListItems = new List<SynergyV5ListItem>();
            ListId = -1; //Load() will populate id if found

            if (!deferredLoad)
            {
                Load();
            }
        }

        public SynergyV5List(string listName, 
                             DateTime activatedAtTime, 
                             DateTime shelvedAtTime) : this(listName, true)
        {
            ActivatedAt = activatedAtTime;
            ShelvedAt = shelvedAtTime;

            Load();
        }

        public string ListName { get; private set; }
        public DateTime ActivatedAt { get; private set; }
        public DateTime ShelvedAt { get; private set; }

        public List<SynergyV5ListItem> ListItems { get; private set; }

        public int ListId { get; private set; }

        public void Load()
        {
            //mock
            ListItems.Add(new SynergyV5ListItem("demo"));

            //should try to retrieve from database, and populate if found

            //be sure to populate ListId, if -1 on Save(), an insert will occur
            //otherwise just an update

            //if not found, do nothing

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //account for this situation:
            //instantiated with the datetime parameter constructor
            // because loaded from xml (for example)
            // load from database will get a conflicting value
            // since either could be more recent, resolve to newest value
            // remember, other constructor could be null datetime,
            // so check for that as well, something overrides nothing
        }
    }
}
