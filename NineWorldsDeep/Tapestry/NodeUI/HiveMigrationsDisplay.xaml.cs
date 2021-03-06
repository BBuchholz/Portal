﻿using NineWorldsDeep.Core;
using NineWorldsDeep.Hive;
using NineWorldsDeep.Hive.Spores;
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

        public void RefreshByTreeView(TreeView tv)
        {
            if(tv == tvHierarchyA)
            {
                RefreshA();
            }

            if(tv == tvHierarchyB)
            {
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

        private TreeView GetOppositeTreeView(TreeView tv)
        {
            if(tv == tvHierarchyA)
            {
                return tvHierarchyB;
            }
            else
            {
                return tvHierarchyA;
            }
        }

        private HiveRoot GetSelectedHiveRootForTreeView(TreeView tv)
        {
            ComboBox cmb;

            if(tv == tvHierarchyA)
            {
                cmb = cmbRootsA;
            }
            else
            {
                cmb = cmbRootsB;
            }

            return (HiveRoot)cmb.SelectedItem;
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
            else if (rootIsStaging)
            {
                item.ContextMenu = (ContextMenu)this.Resources["cmStagingRootLobe"];
            }
            else
            {
                item.ContextMenu = (ContextMenu)this.Resources["cmNonLocalRootLobe"];
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
            else if (rootIsStaging)
            {
                item.ContextMenu = (ContextMenu)this.Resources["cmStagingRootSpore"];
            }
            else
            {
                item.ContextMenu = (ContextMenu)this.Resources["cmNonLocalRootSpore"];
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

        private void btnRefreshA_Click(object sender, RoutedEventArgs e)
        {
            RefreshA();
        }

        private void btnRefreshB_Click(object sender, RoutedEventArgs e)
        {
            RefreshB();
        }

        private void Intake(object sender)
        {
            MenuItem mnu = sender as MenuItem;
            TreeViewItem item = null;
            if (mnu != null)
            {
                item = ((ContextMenu)mnu.Parent).PlacementTarget as TreeViewItem;

                if (item != null && item.Tag != null)
                {
                    TreeView parentTreeView =
                        UtilsUi.ParentOfType<TreeView>(item);

                    List<string> paths = new List<string>();

                    if (item.Tag is HiveSporeFilePath)
                    {
                        HiveSporeFilePath spore = item.Tag as HiveSporeFilePath;
                        paths.Add(spore.FilePath);
                    }

                    if (item.Tag is HiveLobe)
                    {
                        var lobe = item.Tag as HiveLobe;

                        UtilsHive.RefreshSpores(lobe);

                        foreach (HiveSpore spore in lobe.Spores)
                        {
                            if (spore is HiveSporeFilePath)
                            {
                                var fileSpore = spore as HiveSporeFilePath;
                                paths.Add(fileSpore.FilePath);
                            }
                        }
                    }

                    string msg = UtilsHive.Intake(paths);

                    UI.Display.Message(msg);
                    RefreshByTreeView(parentTreeView);
                }
            }
        }

        private void MenuItemIntakeRootSpore_Click(object sender, RoutedEventArgs e)
        {
            Intake(sender);

            //MenuItem mnu = sender as MenuItem;
            //TreeViewItem item = null;
            //if (mnu != null)
            //{
            //    item = ((ContextMenu)mnu.Parent).PlacementTarget as TreeViewItem;

            //    if (item != null && item.Tag != null)
            //    {
            //        HiveSporeFilePath spore = item.Tag as HiveSporeFilePath;
            //        TreeView parentTreeView =
            //            UtilsUi.ParentOfType<TreeView>(item);

            //        if (spore != null)
            //        {
            //            List<string> paths = new List<string>();
            //            paths.Add(spore.FilePath);
            //            string msg = UtilsHive.Intake(paths);

            //            UI.Display.Message(msg);
            //            RefreshByTreeView(parentTreeView);
            //        }
            //    }
            //}
        }

        private void MenuItemCopyToOther_Click(object sender, RoutedEventArgs e)
        {
            ProcessFileMovement(sender, FileTransportOperationType.CopyTo);
        }

        //private void ProcessFileMovementPreviousVersion(object sender, FileTransportOperationType fileTransportType)
        //{
        //    MenuItem mnu = sender as MenuItem;
        //    TreeViewItem item = null;
        //    if (mnu != null)
        //    {
        //        item = ((ContextMenu)mnu.Parent).PlacementTarget as TreeViewItem;

        //        if (item != null && item.Tag != null)
        //        {
        //            HiveSporeFilePath spore = item.Tag as HiveSporeFilePath;
        //            TreeView parentTreeView =
        //                UtilsUi.ParentOfType<TreeView>(item);

        //            if (spore != null)
        //            {
        //                var destinationRoot = 
        //                    GetSelectedHiveRootForTreeView(
        //                        GetOppositeTreeView(parentTreeView));

        //                List<string> pathsToProcess = new List<string>();
        //                pathsToProcess.Add(spore.FilePath);

        //                UtilsHive.ProcessMovement(
        //                    pathsToProcess, 
        //                    destinationRoot, 
        //                    fileTransportType);

        //                UI.Display.Message("file operation processed.");

        //                RefreshA();
        //                RefreshB();
        //            }
        //        }
        //    }
        //}

        private void ProcessFileMovement(object sender, FileTransportOperationType fileTransportType)
        {
            MenuItem mnu = sender as MenuItem;
            TreeViewItem item = null;
            if (mnu != null)
            {
                item = ((ContextMenu)mnu.Parent).PlacementTarget as TreeViewItem;

                if (item != null && item.Tag != null)
                {
                    TreeView parentTreeView =
                        UtilsUi.ParentOfType<TreeView>(item);
                                        
                    var destinationRoot =
                        GetSelectedHiveRootForTreeView(
                            GetOppositeTreeView(parentTreeView));

                    List<string> pathsToProcess = new List<string>();

                    if (item.Tag is HiveSporeFilePath)
                    {
                        HiveSporeFilePath spore = item.Tag as HiveSporeFilePath;

                        pathsToProcess.Add(spore.FilePath);
                    }

                    if (item.Tag is HiveLobe)
                    {
                        HiveLobe lobe = item.Tag as HiveLobe;

                        UtilsHive.RefreshSpores(lobe);

                        foreach(HiveSpore spore in lobe.Spores)
                        {
                            if(spore is HiveSporeFilePath)
                            {
                                var fileSpore = spore as HiveSporeFilePath;
                                pathsToProcess.Add(fileSpore.FilePath);
                            }
                        }
                    }

                    UtilsHive.ProcessMovement(
                        pathsToProcess,
                        destinationRoot,
                        fileTransportType);

                    UI.Display.Message("file operation(s) processed.");

                    RefreshA();
                    RefreshB();
                    
                }
            }
        }


        private void MenuItemMoveToOther_Click(object sender, RoutedEventArgs e)
        {
            ProcessFileMovement(sender, FileTransportOperationType.MoveTo);
        }

        private void MenuItemIntakeAllLocalLobe_Click(object sender, RoutedEventArgs e)
        {
            Intake(sender);
        }

        private void MenuItemCopyAllToOtherStagingLobe_Click(object sender, RoutedEventArgs e)
        {
            ProcessFileMovement(sender, FileTransportOperationType.CopyTo);
        }

        private void MenuItemMoveAllToOtherStagingLobe_Click(object sender, RoutedEventArgs e)
        {
            ProcessFileMovement(sender, FileTransportOperationType.MoveTo);
        }
    }

    public enum HiveMigrationDisplayDestination
    {
        SteadA,
        SteadB
    }

}
