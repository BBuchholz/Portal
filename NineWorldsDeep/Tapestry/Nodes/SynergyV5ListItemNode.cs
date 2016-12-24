using NineWorldsDeep.Core;
using NineWorldsDeep.Synergy.V5;
using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class SynergyV5ListItemNode : TapestryNode
    {
        public SynergyV5ListItemNode(string listName, string itemValue)
            : this(new SynergyV5List(listName), new SynergyV5ListItem(itemValue))
        {
            //do nothing, chained constructor
        }

        public SynergyV5ListItemNode(SynergyV5List lst, SynergyV5ListItem itm)
            : base("SynergyV5ListItem/" +
          Hashes.Sha1ForStringValue(lst.ListName + ": " + itm.ItemValue))
        {
            List = lst;
            Item = itm;
        }

        public SynergyV5ListItem Item { get; private set; }
        public SynergyV5List List { get; private set; }

        public override string GetShortName()
        {
            return List.ListName + ": " + Item.ItemValue;
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            //do nothing
        }

        public override TapestryNodeType NodeType
        {
            get
            {
                return TapestryNodeType.SynergyV5ListItem;
            }
        }
    }
}
