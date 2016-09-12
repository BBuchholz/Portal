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
    /// Interaction logic for ClusterChooser.xaml
    /// </summary>
    public partial class ClusterChooser : UserControl
    {
        private ClusterNode clusterNode;
        
        
        public ClusterChooser()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            clusterNode.Loaded = true;
            OnClusterLoadedStateChanged(
                new ClusterLoadedStateChangedEventArgs(clusterNode));
        }

        public void Display(ClusterNode nd)
        {
            clusterNode = nd;
        }

        protected virtual void OnClusterLoadedStateChanged(
            ClusterLoadedStateChangedEventArgs e)
        {
            ClusterLoadedStateChanged?.Invoke(this, e);
        }

        public event EventHandler<ClusterLoadedStateChangedEventArgs> 
            ClusterLoadedStateChanged;
    }
}
