using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Synergy
{
    public class SynergyList
    {
        private List<SynergyItem> _items =
            new List<SynergyItem>();

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public void AddItem(SynergyItem si)
        {
            bool found = false;

            foreach (SynergyItem item in _items)
            {
                if(si.Item.Equals(item.Item, 
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    item.True(si);
                    found = true;
                }
            }

            if (!found && 
                !string.IsNullOrWhiteSpace(si.Item) &&
                !si.Fragment.Contains("item={}")) //HACK: trying to fix a bug causing "item={item={}}"
            {
                _items.Add(si);
            }
        }

        public IEnumerable<SynergyItem> Items
        {
            get
            {
                return _items;
            }
        }

        public void True(SynergyList sl)
        {
            foreach(SynergyItem si in sl.Items)
            {
                AddItem(si);
            }
        }
    }
}
