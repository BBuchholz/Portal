using NineWorldsDeep.Tapestry.NodeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class HierophantVertexNode : TapestryNode
    {
        public HierophantUiCoupling Coupling { get; private set; }

        public HierophantVertexNode(HierophantUiCoupling coupling)
            : base("HierophantVertex/" + coupling.Vertex.NameId)
        {
            Coupling = coupling;
        }

        protected HierophantVertexNode()
            : base("HierophantVertex/NULL")
        {
            Coupling = null;
        }

        public override string GetShortName()
        {
            return Coupling.Vertex.NameId;
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
                return TapestryNodeType.HierophantVertex;
            }
        }

    }
}
