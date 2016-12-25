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
        public string ItemStatus
        {
            get
            {
                if (ToDo == null)
                {
                    return "Permanent";
                }

                return ToDo.Status;
            }
        }

        internal void Activate()
        {
            if(ToDo == null)
            {
                ToDo = new SynergyV5ToDo();
            }

            ToDo.Activate();
        }

        internal void Archive()
        {
            if (ToDo == null)
            {
                ToDo = new SynergyV5ToDo();
            }

            ToDo.Archive();
        }

        internal void Complete()
        {
            if (ToDo == null)
            {
                ToDo = new SynergyV5ToDo();
            }

            ToDo.Complete();
        }
    }
}