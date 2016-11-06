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
    /// Interaction logic for SynergyV5ListDisplay.xaml
    /// </summary>
    public partial class SynergyV5ListDisplay : UserControl
    {
        public SynergyV5ListDisplay()
        {
            InitializeComponent();
        }

        private void lvSynergyV5ListItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SynergyV5ListItemNode nd = 
                (SynergyV5ListItemNode)lvSynergyV5ListItems.SelectedItem;

            if(nd != null)
            {
                SynergyV5ListItemClickedEventArgs args =
                    new SynergyV5ListItemClickedEventArgs(nd);

                OnSynergyV5ListItemClicked(args);
            }
        }

        protected virtual void OnSynergyV5ListItemClicked(
            SynergyV5ListItemClickedEventArgs args)
        {
            SynergyV5ListItemClicked?.Invoke(this, args);
        }

        public event EventHandler<SynergyV5ListItemClickedEventArgs> SynergyV5ListItemClicked;

        public class SynergyV5ListItemClickedEventArgs
        {
            public SynergyV5ListItemClickedEventArgs(SynergyV5ListItemNode nd)
            {
                ListItemNode = nd;
            }

            public SynergyV5ListItemNode ListItemNode { get; private set; }
        }
    }
}
