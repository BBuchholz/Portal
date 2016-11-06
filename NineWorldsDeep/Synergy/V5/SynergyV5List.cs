using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Synergy.V5
{
    public class SynergyV5List
    {
        public SynergyV5List(string listName)
        {
            ListName = listName;
            ListItems = new List<SynergyV5ListItem>();
            ListId = -1; //Load() will populate id if found

            Load();
        }

        public string ListName { get; private set; }

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
        }
    }
}
