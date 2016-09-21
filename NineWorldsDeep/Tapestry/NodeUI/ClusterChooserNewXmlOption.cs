using System;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    internal class ClusterChooserNewXmlOption : ClusterChooserOption
    {
        public override string OptionText
        {
            get
            {
                return "New Xml Cluster...";
            }
        }

        public override ClusterNode Retrieve()
        {
            return null;
        }
    }
}