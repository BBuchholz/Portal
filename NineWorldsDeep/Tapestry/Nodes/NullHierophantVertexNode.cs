using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class NullHierophantVertexNode : HierophantVertexNode
    {
        public override TapestryNodeType NodeType
        {
            get
            {
                return TapestryNodeType.NullHierophantVertex;
            }
        }
    }
}
