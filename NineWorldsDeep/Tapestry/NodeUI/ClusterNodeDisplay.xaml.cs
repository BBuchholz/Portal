using NineWorldsDeep.Tapestry.Nodes;
using System.Windows;
using System.Windows.Controls;
using System;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for ClusterNodeDisplay.xaml
    /// </summary>
    public partial class ClusterNodeDisplay : UserControl
    {
        //private ClusterNode clusterNode;

        public ClusterNodeDisplay()
        {
            InitializeComponent();
            clusterChooser.ClusterLoadedStateChanged += ClusterLoaded_StateChanged;
            clusterDisplay.ClusterLoadedStateChanged += ClusterLoaded_StateChanged;
        }

        private void ClusterLoaded_StateChanged(object sender, ClusterLoadedStateChangedEventArgs e)
        {
            Display(e.ClusterNode);
        }

        public void Display(ClusterNode nd)
        {
            //clusterNode = nd;

            if (!nd.Loaded)
            {
                //display chooser
                //on selection in chooser, cluster will be loaded and display will be called again
                ccClusterDisplay.Visibility = Visibility.Collapsed;
                ccClusterChooser.Visibility = Visibility.Visible;
                clusterChooser.Display(nd);
            }
            else
            {
                //display cluster
                //from cluster, load supported node types and details, load nodes on node type selection change
                ccClusterChooser.Visibility = Visibility.Collapsed;
                ccClusterDisplay.Visibility = Visibility.Visible;
                clusterDisplay.Display(nd);
            }
        }
    }
}
