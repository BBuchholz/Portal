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
    /// Interaction logic for HiveMigrationsDisplay.xaml
    /// </summary>
    public partial class HiveMigrationsDisplay : UserControl
    {
        public HiveMigrationRootNode HiveMigrationRootNode { get; set; }

        public HiveMigrationsDisplay()
        {
            InitializeComponent();
        }

        internal void Display(HiveMigrationRootNode hiveRootNode)
        {
            HiveMigrationRootNode = hiveRootNode;

            Refresh();
        }

        public void Refresh()
        {
            var hr = HiveMigrationRootNode.HiveRoot;


            lblTest.Content = hr.HiveRootName + " received into " +  
                HiveMigrationRootNode.Destination.ToString();
        }

    }
    
    public enum HiveMigrationDisplayDestination
    {
        SteadA,
        SteadB
    }

}
