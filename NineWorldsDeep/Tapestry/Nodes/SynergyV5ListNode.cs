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
            : this(new SynergyV5List(listName))
        {
            //chained constructor, do nothing
        }

        public SynergyV5ListNode(SynergyV5List lst)
            : base("SynergyV5List/" + lst.ListName)
        {
            List = lst;
        }
                
        protected SynergyV5ListNode()
            : base("SynergyV5List/NULL")
        {
            List = null;
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
