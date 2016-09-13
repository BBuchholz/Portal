using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for ClusterDisplay.xaml
    /// </summary>
    public partial class ClusterDisplay : UserControl
    {
        private ClusterNode clusterNode;

        public ClusterDisplay()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            clusterNode.Clear();
            OnClusterLoadedStateChanged(
                new ClusterLoadedStateChangedEventArgs(clusterNode));
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            //string planning = "Should load another ClusterNodeDisplay in the facing pane. " +
            //    " USE EVENT LIKE TapestryNodeViewControl!";
            //UI.Display.Message(planning);

            ClusterDisplayRequestedEventArgs args =
                    new ClusterDisplayRequestedEventArgs(new ClusterNode());

            OnClusterDisplayRequested(args);
        }

        private void NodeTypesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //should load all nodes of selected type in cluster to NodesListView 
        }

        private void NodesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //clicked node should open in facing pane
        }

        public void Display(ClusterNode nd)
        {
            clusterNode = nd;
            ClusterDetailsControl.Display(nd);
        }

        protected virtual void OnClusterLoadedStateChanged(
            ClusterLoadedStateChangedEventArgs e)
        {
            ClusterLoadedStateChanged?.Invoke(this, e);
        }

        protected virtual void OnClusterDisplayRequested(
            ClusterDisplayRequestedEventArgs e)
        {
            ClusterDisplayRequested?.Invoke(this, e);
        }

        public event EventHandler<ClusterDisplayRequestedEventArgs>
            ClusterDisplayRequested;

        public event EventHandler<ClusterLoadedStateChangedEventArgs>
            ClusterLoadedStateChanged;

    }
}
