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
            clusterNode = null;
            OnClusterLoadedStateChanged(
                new ClusterLoadedStateChangedEventArgs(clusterNode));
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            //string planning = "Should load another ClusterNodeDisplay in the facing pane. " +
            //    " USE EVENT LIKE TapestryNodeViewControl!";
            //UI.Display.Message(planning);

            ClusterDisplayRequestedEventArgs args =
                    new ClusterDisplayRequestedEventArgs(new NullClusterNode());

            OnClusterDisplayRequested(args);
        }

        private void NodeTypesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //should load all nodes of selected type in cluster to NodesListView 

            TapestryNodeType nodeType = (TapestryNodeType)NodeTypesComboBox.SelectedItem;
            
            if(clusterNode != null)
            {
                NodesListView.ItemsSource = clusterNode.Children(nodeType);
            }
        }

        private void NodesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //raise node display requested event, subscribers can decide what to do with it
            TapestryNode nd = (TapestryNode) NodesListView.SelectedItem;

            if(nd != null)
            {
                NodeDisplayRequestedEventArgs args =
                    new NodeDisplayRequestedEventArgs(nd);

                OnNodeDisplayRequested(args);
            }
        }

        public void Display(ClusterNode nd)
        {
            clusterNode = nd;
            ClusterDetailsControl.Display(nd);
            PopulateNodeTypes();
        }

        private void PopulateNodeTypes()
        {
            List<TapestryNodeType> lst =
                new List<TapestryNodeType>();

            //TODO: if we can refactor TapestryNodeType to move display types (see TapestryNodeType comments) into their own enum, then we can just iterate through the enum here and any added in the future will just be here
            lst.Add(TapestryNodeType.Audio);
            lst.Add(TapestryNodeType.Image);
            lst.Add(TapestryNodeType.DevicePath);
            lst.Add(TapestryNodeType.Device);
            lst.Add(TapestryNodeType.SynergyList);
            lst.Add(TapestryNodeType.SynergyListItem);
            lst.Add(TapestryNodeType.LyricBit);

            NodeTypesComboBox.ItemsSource = lst;
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

        protected virtual void OnNodeDisplayRequested(
            NodeDisplayRequestedEventArgs e)
        {
            NodeDisplayRequested?.Invoke(this, e);
        }

        public event EventHandler<ClusterDisplayRequestedEventArgs>
            ClusterDisplayRequested;

        public event EventHandler<ClusterLoadedStateChangedEventArgs>
            ClusterLoadedStateChanged;
        
        public event EventHandler<NodeDisplayRequestedEventArgs>
            NodeDisplayRequested;
    }
}
