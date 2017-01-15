using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    class NullSynergyV5ListNode : SynergyV5ListNode
    {
        public override TapestryNodeType NodeType
        {
            get
            {
                return TapestryNodeType.NullSynergyV5List;
            }
        }
    }
}
