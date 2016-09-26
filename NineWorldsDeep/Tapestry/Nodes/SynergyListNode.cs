using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineWorldsDeep.Core;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class SynergyListNode : TapestryNode
    {
        private List<SynergyItemNode> listNodes =
            new List<SynergyItemNode>();

        private SynergyListNode(string uri, params TapestryNode[] children)
            : base(uri, children)
        {
            //use public constructor, chains to this
        }

        public SynergyListNode(string listName)
            : this("Synergy/Lists/" + listName, null)
        {
            ListName = listName;
        }

        public string ListName { get; internal set; }

        public override string GetShortName()
        {
            throw new NotImplementedException();
        }

        public override bool Parallels(TapestryNode nd)
        {
            if (nd is SynergyListNode)
            {
                var lst = nd as SynergyListNode;

                if (lst.ListName.Equals(ListName, StringComparison.CurrentCultureIgnoreCase))
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

        internal void Add(SynergyItemNode synergyItemNode)
        {
            listNodes.AddIdempotent(synergyItemNode);
        }
    }
}
