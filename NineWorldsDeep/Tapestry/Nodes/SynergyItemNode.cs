using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class SynergyItemNode : TapestryNode
    {
        public string ItemText { get; private set; }
        public SynergyListNode ListNode { get; private set; }
        public string ListName { get { return ListNode.ListName; } }

        private SynergyItemNode(string uri)
            : base(uri)
        {
            //use public constructor (chains to this)
        }

        public SynergyItemNode(SynergyListNode lst, string itemText)
            : this(lst.URI + "/" + Hashes.Sha1ForStringValue(itemText))
        {
            ListNode = lst;
            ItemText = itemText;
        }

        public override string GetShortName()
        {
            throw new NotImplementedException();
        }

        public override bool Parallels(TapestryNode nd)
        {
            if(nd is SynergyItemNode)
            {
                var sin = nd as SynergyItemNode;
                
                if(sin.ItemText.Equals(ItemText, StringComparison.CurrentCultureIgnoreCase) &&
                    sin.ListNode.Parallels(ListNode))
                {
                    return true;
                }
            }

            return false;
        }

        public override void PerformSelectionAction()
        {
            throw new NotImplementedException();
        }
    }
}
