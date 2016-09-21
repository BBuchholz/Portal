using System;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    internal class ClusterChooserMockDbOption : ClusterChooserOption
    {
        public override string OptionText
        {
            get
            {
                return "Mock Db Cluster";
            }
        }

        public override ClusterNode Retrieve()
        {
            return new Mock.MockClusterDb();
        }
    }
}