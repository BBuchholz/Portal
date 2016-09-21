using System;
using System.Collections.Generic;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    internal class ClusterRegistry
    {
        List<ClusterChooserOption> chooserOptions =
            new List<ClusterChooserOption>();

        private static ClusterRegistry instance = null;

        private ClusterRegistry()
        {
            PopulateChooserOptions();
        }

        private void PopulateChooserOptions()
        {
            chooserOptions.Add(new ClusterChooserNewDbV5Option());
            chooserOptions.Add(new ClusterChooserNewDbV4cOption());
            chooserOptions.Add(new ClusterChooserNewXmlOption());
            chooserOptions.Add(new ClusterChooserMockXmlOption());
            chooserOptions.Add(new ClusterChooserMockDbOption());
        }

        internal static ClusterRegistry GetInstance()
        {
            if(instance == null)
            {
                instance = new ClusterRegistry();
            }

            return instance;
        }

        public List<ClusterChooserOption> ChooserOptions
        {
            get { return chooserOptions; }
        }
    }
}