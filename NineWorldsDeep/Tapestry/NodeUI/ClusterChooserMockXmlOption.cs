using System;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    internal class ClusterChooserMockXmlOption : ClusterChooserOption
    {
        public override string OptionText
        {
            get
            {
                return "Mock Xml Cluster";
            }
        }

        public override ClusterNode Retrieve()
        {
            return new Mock.MockClusterFileSubsetXml();
        }
    }
}