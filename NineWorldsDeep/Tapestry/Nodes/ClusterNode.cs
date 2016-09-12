using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Tapestry.Nodes
{
    public class ClusterNode : TapestryNode
    {
        public ClusterNode() : 
            base("Cluster")
        {
            Loaded = false;
        }

        public bool Loaded;

        public override string GetShortName()
        {
            return "Cluster";
        }

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

        public void Clear()
        {
            Loaded = false;
        }
    }
}
