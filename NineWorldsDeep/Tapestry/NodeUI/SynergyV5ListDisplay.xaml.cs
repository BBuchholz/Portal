using NineWorldsDeep.Synergy.V5;
using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections;
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
        public static string STATUS_ALL = "All";

        private Db.Sqlite.SynergyV5SubsetDb db;
        private SynergyV5ListNode listNode;

        public SynergyV5ListDisplay()
        {
            InitializeComponent();
            db = new Db.Sqlite.SynergyV5SubsetDb();

            LoadStatusValues();
        }

        private void LoadStatusValues()
        {
            List<string> statuses = new List<string>();

            statuses.Add(SynergyV5ListItem.LIST_ITEM_STATUS_PERMANENT);            
            statuses.Add(SynergyV5ToDo.TO_DO_STATUS_ACTIVATED);
            statuses.Add(SynergyV5ToDo.TO_DO_STATUS_ARCHIVED);
            statuses.Add(SynergyV5ToDo.TO_DO_STATUS_COMPLETED);
            statuses.Add(SynergyV5ToDo.TO_DO_STATUS_INDETERMINATE);
            statuses.Add(STATUS_ALL);

            cmbItemStatusFilter.ItemsSource = null;
            cmbItemStatusFilter.ItemsSource = statuses;
        }

        private void lvSynergyV5ListItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SynergyV5ListItem item = 
                (SynergyV5ListItem)lvSynergyV5ListItems.SelectedItem;

            if(item != null)
            {
                SynergyV5ListItemNode nd =
                    new SynergyV5ListItemNode(listNode.List, item);
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

        internal void Display(SynergyV5ListNode listNode)
        {
            this.listNode = listNode;

            Refresh();
        }

        public void Refresh()
        {
            SynergyV5List synLst = this.listNode.List;

            tbListName.Text = "";
            tbListStatus.Text = "";
            lvSynergyV5ListItems.ItemsSource = null; //clear existing

            if (synLst != null)
            {
                synLst.Sync(db); //syncs with db, loads/merges

                tbListName.Text = synLst.ListName;
                tbListStatus.Text = synLst.Status;

                string statusValue = (string) cmbItemStatusFilter.SelectedItem;

                List<SynergyV5ListItem> filteredItems;

                if (string.IsNullOrWhiteSpace(statusValue))
                {
                    statusValue = STATUS_ALL;
                }

                if (statusValue.Equals(STATUS_ALL))
                {
                    filteredItems = synLst.ListItems;
                }
                else
                {
                    filteredItems = synLst.ListItems.Where(sli => sli.ItemStatus.Contains(statusValue)).ToList();
                }

                //display list here
                lvSynergyV5ListItems.ItemsSource = filteredItems;
            }
        }

        private int GetSelectedPermanentItemsCount()
        {
            int permCount = 0;

            IList items = (IList)lvSynergyV5ListItems.SelectedItems;
            var selectedItems = items.Cast<SynergyV5ListItem>();

            foreach (SynergyV5ListItem sli in selectedItems)
            {
                if (sli.IsPermanent)
                {
                    permCount++;
                }
            }

            return permCount;
        }
        
        private void MenuItemActivateSelected_Click(object sender, RoutedEventArgs e)
        {
            IList items = (IList)lvSynergyV5ListItems.SelectedItems;
            var selectedItems = items.Cast<SynergyV5ListItem>();

            bool proceed = true;

            int permCount = GetSelectedPermanentItemsCount();

            if(permCount > 0)
            {
                proceed = false;

                string msg = "Are you sure you want to Activate these " +
                    selectedItems.Count() + " items? " +
                    permCount + " are permanent items and CANNOT be reverted.";

                if (UI.Prompt.Confirm(msg, true))
                {
                    proceed = true;
                }
            }

            if (proceed)
            {
                foreach (SynergyV5ListItem sli in selectedItems)
                {
                    sli.Activate();
                }

                this.listNode.List.Sync(db);

                Refresh();
            }
        }

        private void MenuItemCompleteSelected_Click(object sender, RoutedEventArgs e)
        {
            IList items = (IList)lvSynergyV5ListItems.SelectedItems;
            var selectedItems = items.Cast<SynergyV5ListItem>();

            bool proceed = true;

            int permCount = GetSelectedPermanentItemsCount();

            if (permCount > 0)
            {
                proceed = false;

                string msg = "Are you sure you want to Complete these " +
                    selectedItems.Count() + " items? " +
                    permCount + " are permanent items and CANNOT be reverted.";

                if (UI.Prompt.Confirm(msg, true))
                {
                    proceed = true;
                }
            }

            if (proceed)
            {
                foreach (SynergyV5ListItem sli in selectedItems)
                {
                    sli.Complete();
                }

                this.listNode.List.Sync(db);

                Refresh();
            }            
        }

        private void MenuItemArchiveSelected_Click(object sender, RoutedEventArgs e)
        {
            IList items = (IList)lvSynergyV5ListItems.SelectedItems;
            var selectedItems = items.Cast<SynergyV5ListItem>();

            bool proceed = true;

            int permCount = GetSelectedPermanentItemsCount();

            if (permCount > 0)
            {
                proceed = false;

                string msg = "Are you sure you want to Archive these " +
                    selectedItems.Count() + " items? " +
                    permCount + " are permanent items and CANNOT be reverted.";

                if (UI.Prompt.Confirm(msg, true))
                {
                    proceed = true;
                }
            }

            if (proceed)
            {
                foreach (SynergyV5ListItem sli in selectedItems)
                {
                    sli.Archive();
                }

                this.listNode.List.Sync(db);

                Refresh();
            }
        
        }

        private void btnCreateListItem_Click(object sender, RoutedEventArgs e)
        {
            string itemValue = txtListItemValueEntry.Text;

            if (!string.IsNullOrWhiteSpace(itemValue))
            {
                if(CurrentList != null)
                {
                    CurrentList.Add(0, new SynergyV5ListItem(itemValue));
                    CurrentList.Sync(db);
                    Refresh();

                    txtListItemValueEntry.Text = "";
                }
            }
        }

        private SynergyV5List CurrentList
        {
            get
            {
                if(this.listNode != null)
                {
                    return this.listNode.List;
                }

                return null;
            }
        }

        private void cmbItemStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }
    }
}
