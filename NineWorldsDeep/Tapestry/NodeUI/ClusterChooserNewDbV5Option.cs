using System;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    internal class ClusterChooserNewDbV5Option : ClusterChooserOption
    {
        public override string OptionText
        {
            get
            {
                return "New Db V5 Cluster...";
            }
        }

        public override ClusterNode Retrieve()
        {
            return null;
        }
    }
}