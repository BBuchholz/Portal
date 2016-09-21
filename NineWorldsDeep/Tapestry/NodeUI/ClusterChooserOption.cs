using System;
using System.ComponentModel;
using NineWorldsDeep.Tapestry.Nodes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    public abstract class ClusterChooserOption
    {
        public abstract string OptionText { get; }

        public override string ToString()
        {
            return OptionText;
        }

        public abstract ClusterNode Retrieve();
    }
}