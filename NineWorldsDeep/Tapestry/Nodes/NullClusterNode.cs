using System;
using System.Collections.Generic;

namespace NineWorldsDeep.Tapestry.Nodes
{
    internal class NullClusterNode : ClusterNode
    {
        public NullClusterNode()
            : base("Clusters")
        {
        }

        public override string GetShortName()
        {
            //loaded as chooser
            return "Clusters";
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

        public override bool Parallels(TapestryNode nd)
        {
            throw new NotImplementedException();
        }

        public override TapestryNodeType NodeType
        {
            get
            {
                return TapestryNodeType.NullCluster;
            }
        }
    }
}