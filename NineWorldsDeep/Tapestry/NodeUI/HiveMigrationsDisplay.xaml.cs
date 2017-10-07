using NineWorldsDeep.Hive;
using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<HiveRoot> HiveRootsA { get; set; }
        public ObservableCollection<HiveRoot> HiveRootsB { get; set; }

        public HiveMigrationsDisplay()
        {
            InitializeComponent();
            HiveRootsA = new ObservableCollection<HiveRoot>();
            HiveRootsB = new ObservableCollection<HiveRoot>();

            this.DataContext = this;
        }

        internal void Display(HiveMigrationRootNode hiveRootNode)
        {
            var hr = hiveRootNode.HiveRoot;

            if (hiveRootNode.Destination == HiveMigrationDisplayDestination.SteadA)
            {
                if (!HiveRootsA.Contains(hr))
                {
                    HiveRootsA.Add(hr);
                }

                cmbRootsA.SelectedItem = hr;
                RefreshA();
            }
            else
            {
                if (!HiveRootsB.Contains(hr))
                {
                    HiveRootsB.Add(hr);
                }

                cmbRootsB.SelectedItem = hr;
                RefreshB();
            }
        }

        public void RefreshA()
        {
            Refresh(cmbRootsA, tvHierarchyA);
        }

        public void RefreshB()
        {
            Refresh(cmbRootsB, tvHierarchyB);
        }

        private void Refresh(ComboBox cmb, TreeView tv)
        {
            var hr = (HiveRoot)cmb.SelectedItem;

            if(hr != null && hr is HiveRoot)
            {
                UtilsHive.RefreshLobes(hr);

                PopulateLobesTreeView(hr, tv);
            }
        }

        private void PopulateLobesTreeView(HiveRoot hr, TreeView tv)
        {
            tv.Items.Clear();

            foreach (HiveLobe hl in hr.Lobes)
            {
                tv.Items.Add(CreateTreeItem(hl));
            }
        }

        private void SelectRightClickedTreeViewItem(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem)e.Source;
            item.IsSelected = true;
        }

        private TreeViewItem CreateTreeItem(HiveLobe hl)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = hl.HiveLobeName;
            item.Tag = hl;
            item.Items.Add("Loading...");
            
            bool rootIsLocal = UtilsHive.IsLocalRoot(hl.HiveRoot);
            bool rootIsStaging = UtilsHive.IsStagingRoot(hl.HiveRoot);

            if (rootIsLocal)
            {
                item.ContextMenu = (ContextMenu)this.Resources["cmLocalRootLobe"];
            }

            if (rootIsStaging)
            {
                item.ContextMenu = (ContextMenu)this.Resources["cmStagingRootLobe"];
            }

            item.MouseRightButtonDown += SelectRightClickedTreeViewItem;
            return item;
        }

        private TreeViewItem CreateTreeItem(HiveSpore hs)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = hs.Name;
            item.Tag = hs;
            item.Items.Add("Loading...");
            
            bool rootIsLocal = UtilsHive.IsLocalRoot(hs.HiveLobe.HiveRoot);
            bool rootIsStaging = UtilsHive.IsStagingRoot(hs.HiveLobe.HiveRoot);

            if (rootIsLocal)
            {
                item.ContextMenu = (ContextMenu)this.Resources["cmLocalRootSpore"];
            }

            if (rootIsStaging)
            {
                item.ContextMenu = (ContextMenu)this.Resources["cmStagingRootSpore"];
            }
            
            item.MouseRightButtonDown += SelectRightClickedTreeViewItem;
            return item;
        }

        private void MenuItemTest_Click(object sender, RoutedEventArgs e)
        {
            string msg = "null tag";

            MenuItem mnu = sender as MenuItem;
            TreeViewItem item = null;
            if (mnu != null)
            {
                item = ((ContextMenu)mnu.Parent).PlacementTarget as TreeViewItem;

                if (item != null && item.Tag != null)
                {
                    msg = item.Tag.ToString();
                }
            }

            UI.Display.Message(msg);
        }

        private void tvHierarchyA_Expanded(object sender, RoutedEventArgs e)
        {
            ProcessExpander(e);
        }

        private void tvHierarchyB_Expanded(object sender, RoutedEventArgs e)
        {
            ProcessExpander(e);
        }

        private void ProcessExpander(RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;

            if(item.Items.Count == 1 &&
                item.Items[0] is string)
            {
                item.Items.Clear();

                if(item.Tag is HiveLobe)
                {
                    HiveLobe hl = item.Tag as HiveLobe;

                    UtilsHive.RefreshSpores(hl);

                    foreach (HiveSpore hs in hl.Spores)
                    {
                        item.Items.Add(CreateTreeItem(hs));
                    }
                }
                
                if (item.Tag is HiveSpore)
                {
                    //do something
                }
            }
        }


    }

    public enum HiveMigrationDisplayDestination
    {
        SteadA,
        SteadB
    }

}
