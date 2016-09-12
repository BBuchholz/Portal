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
    /// Interaction logic for ClusterDetails.xaml
    /// </summary>
    public partial class ClusterDetails : UserControl
    {
        private ClusterNode clusterNode;
        
        public ClusterDetails()
        {
            InitializeComponent();
        }

        public void Display(ClusterNode nd)
        {
            clusterNode = nd;
            MultiLineTextBox.Text = nd.ToMultiLineDetail();            
        }
    }
}
