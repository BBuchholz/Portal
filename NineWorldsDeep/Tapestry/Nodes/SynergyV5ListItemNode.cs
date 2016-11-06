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
            : base("SynergyV5ListItem/" + 
                  Hashes.Sha1ForStringValue(listName + ": " + itemValue))
        {
            Item = new SynergyV5ListItem(itemValue);
            List = new SynergyV5List(listName);
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
