using NineWorldsDeep.Synergy.V5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class SynergyV5ListNode : TapestryNode
    {
        public SynergyV5ListNode(string listName)
            : base("SynergyV5List/" + listName)
        {
            List = new SynergyV5List(listName);
        }

        public SynergyV5List List { get; private set; }

        public override string GetShortName()
        {
            return List.ListName;
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
                return TapestryNodeType.SynergyV5List;
            }
        }
    }
}
