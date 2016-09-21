using System;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    internal class ClusterChooserNewDbV4cOption : ClusterChooserOption
    {
        public override string OptionText
        {
            get
            {
                return "New Db V4 Cluster...";
            }
        }

        public override ClusterNode Retrieve()
        {
            return null;
        }
    }
}