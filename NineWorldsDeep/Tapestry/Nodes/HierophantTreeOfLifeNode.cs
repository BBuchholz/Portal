using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class HierophantTreeOfLifeNode : TapestryNode
    {
        public HierophantTreeOfLifeNode()
            : base("Hierophant/TreeOfLife")
        {
        }

        public override string GetShortName()
        {
            return "Hierophant Tree Of Life";
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
                return TapestryNodeType.HierophantTreeOfLife;
            }
        }
    }
}
