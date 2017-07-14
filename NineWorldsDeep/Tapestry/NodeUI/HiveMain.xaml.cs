using NineWorldsDeep.Hive;
using NineWorldsDeep.Mnemosyne.V5;
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
    /// Interaction logic for HiveMain.xaml
    /// </summary>
    public partial class HiveMain : UserControl
    {
        public HiveMain()
        {
            InitializeComponent();
            PopulateRoots(UtilsHive.GetAllRoots());
        }

        
        private void PopulateRoots(List<HiveRoot> allRoots)
        {
            foreach (HiveRoot hr in allRoots)
            {
                tvHive.Items.Add(CreateTreeItem(hr));
            }
        }

        private void tvHive_Expanded(object sender, RoutedEventArgs e)
        {
            ProcessExpander(e);
        }

        private void ProcessExpander(RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;

            if (item.Items.Count == 1 &&
                item.Items[0] is string)
            {
                item.Items.Clear();

                if(item.Tag is HiveRoot)
                {
                    HiveRoot hr = item.Tag as HiveRoot;

                    UtilsHive.RefreshLobes(hr);

                    foreach(HiveLobe hl in hr.Lobes)
                    {
                        item.Items.Add(CreateTreeItem(hl));
                    }
                }

                if(item.Tag is HiveLobe)
                {
                    HiveLobe hl = item.Tag as HiveLobe;

                    UtilsHive.RefreshSpores(hl);

                    foreach(HiveSpore hs in hl.Spores)
                    {
                        item.Items.Add(CreateTreeItem(hs));
                    }
                }

                if(item.Tag is HiveSpore)
                {
                    //do something
                }
            }
        }
        
        private TreeViewItem CreateTreeItem(HiveRoot hr)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = hr.Name;
            item.Tag = hr;
            item.Items.Add("Loading...");
            return item;
        }

        private TreeViewItem CreateTreeItem(HiveSpore hs)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = hs.Name;
            item.Tag = hs;
            item.Items.Add("Loading...");
            return item;
        }

        private TreeViewItem CreateTreeItem(HiveLobe hl)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = hl.Name;
            item.Tag = hl;
            item.Items.Add("Loading...");
            return item;
        }
        
    }
}
