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

        public List<SynergyItem> Items
        {
            get
            {
                return _items;
            }
        }
    }
}
