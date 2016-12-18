using System;

namespace NineWorldsDeep.Synergy.V5
{
    public class SynergyV5ListItem
    {
        public SynergyV5ListItem(string itemValue)
        {
            ItemValue = itemValue;
            ItemId = -1;
            ListItemId = -1;
        }

        public SynergyV5ListItem(string itemValue, 
                                 DateTime? itemActivatedAtTime, 
                                 DateTime? completedAtTime, 
                                 DateTime? archivedAtTime) : this(itemValue)
        {
            SynergyV5ToDo newToDo = new SynergyV5ToDo();
            newToDo.SetTimeStamps(itemActivatedAtTime, completedAtTime, archivedAtTime);
            ToDo = newToDo;
        }
        
        public string ItemValue { get; private set; }
        public int ItemId { get; set; }
        public int ListItemId { get; set; }
        public SynergyV5ToDo ToDo { get; set; }

    }
}