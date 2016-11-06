using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class SynergyV5MasterListNode : TapestryNode
    {
        public SynergyV5MasterListNode()
            : base("Synergy/MasterList")
        {
        }

        public override string GetShortName()
        {
            return "Synergy V5";
        }

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override void PerformSelectionAction()
        {
            //do nothing
        }

        public override IEnumerable<TapestryNode> Children(TapestryNodeType nodeType)
        {
            //always empty
            return new List<TapestryNode>();
        }

        public override TapestryNodeType NodeType
        {
            get
            {
                return TapestryNodeType.SynergyV5MasterList;
            }
        }
    }
}
