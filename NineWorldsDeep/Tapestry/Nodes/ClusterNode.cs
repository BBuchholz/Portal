using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public abstract class ClusterNode : TapestryNode
    {
        public ClusterNode(string uri, params TapestryNode[] children) 
            : base(uri, children)
        {
        }

        public override abstract string GetShortName();
        //{
        //    return "Cluster";
        //}

        public override void PerformSelectionAction()
        {
            //do nothing (for now?)
        }

        public override TapestryNodeType NodeType
        {
            get
            {
                return TapestryNodeType.Cluster;
            }
        }

        public override abstract IEnumerable<TapestryNode> Children(TapestryNodeType nodeType);
    }
}
