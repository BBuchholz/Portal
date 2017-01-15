using NineWorldsDeep.Core;
using NineWorldsDeep.Synergy.V5;
using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Xml.Linq;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for SynergyV5MasterListDisplay.xaml
    /// </summary>
    public partial class SynergyV5MasterListDisplay : UserControl
    {
        Db.Sqlite.SynergyV5SubsetDb db;

        //async related 
        private readonly SynchronizationContext syncContext;
        private DateTime previousTime = DateTime.Now;

        public SynergyV5MasterListDisplay()
        {
            InitializeComponent();
            syncContext = SynchronizationContext.Current;

            db = new Db.Sqlite.SynergyV5SubsetDb();
            
            Load();

            statusDetail.Text = "Ready";
        }

        private void Load()
        {
            List<SynergyV5ListNode> activeLists =
                new List<SynergyV5ListNode>();

            //clean listNames of timestamps
            foreach (string listName in db.GetAllActiveListNames())
            {
                if (TimeStamp.ContainsTimeStamp_YYYYMMDD(listName))
                {
                    //create and sync (will handle shelving, renaming, etc.
                    SynergyV5List synLst = new SynergyV5List(listName);

                    synLst.Sync(db);
                }
            }

            foreach (string listName in db.GetAllActiveListNames())
            {
                activeLists.Add(new SynergyV5ListNode(listName));
            }

            lvSynergyV5ActiveLists.ItemsSource = null; //reset value
            lvSynergyV5ActiveLists.ItemsSource = activeLists;

            List<SynergyV5ListNode> shelvedLists =
                new List<SynergyV5ListNode>();

            foreach (string listName in db.GetAllShelvedListNames())
            {
                shelvedLists.Add(new SynergyV5ListNode(listName));
            }

            lvSynergyV5ShelvedLists.ItemsSource = null; //reset value
            lvSynergyV5ShelvedLists.ItemsSource = shelvedLists;
        }

        private void lvSynergyV5Lists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //mirrors ChordProgressionsNodeDisplay
            SynergyV5ListNode nd = (SynergyV5ListNode)lvSynergyV5ActiveLists.SelectedItem;

            if(nd != null)
            {
                SynergyV5ListClickedEventArgs args =
                    new SynergyV5ListClickedEventArgs(nd);

                OnSynergyV5ListClicked(args);
            }

            ProcessNullListSelection();
        }

        private void ProcessNullListSelection()
        {
            if(lvSynergyV5ActiveLists.SelectedItem == null &&
                lvSynergyV5ShelvedLists.SelectedItem == null)
            {

                SynergyV5ListClickedEventArgs args =
                    new SynergyV5ListClickedEventArgs(new NullSynergyV5ListNode());

                OnSynergyV5ListClicked(args);
            }

        }

        private void lvSynergyV5ShelvedLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //mirrors ChordProgressionsNodeDisplay
            SynergyV5ListNode nd = (SynergyV5ListNode)lvSynergyV5ShelvedLists.SelectedItem;

            if (nd != null)
            {
                SynergyV5ListClickedEventArgs args =
                    new SynergyV5ListClickedEventArgs(nd);

                OnSynergyV5ListClicked(args);
            }

            ProcessNullListSelection();
        }

        protected virtual void OnSynergyV5ListClicked(SynergyV5ListClickedEventArgs args)
        {
            SynergyV5ListClicked?.Invoke(this, args);
        }

        public event EventHandler<SynergyV5ListClickedEventArgs> SynergyV5ListClicked;

        public class SynergyV5ListClickedEventArgs
        {
            public SynergyV5ListClickedEventArgs(SynergyV5ListNode nd)
            {
                ListNode = nd;
            }

            public SynergyV5ListNode ListNode { get; private set; }

        }

        private void btnCreateList_Click(object sender, RoutedEventArgs e)
        {
            string listName = 
                Synergy.SynergyUtils.ProcessListName(txtListNameEntry.Text);

            if (!string.IsNullOrWhiteSpace(listName))
            {
                SynergyV5List synLst = new SynergyV5List(listName);
                synLst.Sync(db);

                Load();

                txtListNameEntry.Text = "";
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            ProcessExpanderState((Expander)sender);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            ProcessExpanderState((Expander)sender);
        }

        /// <summary>
        /// manages grid rows to share space between multiple expanded expanders
        /// </summary>
        /// <param name="expander"></param>
        private void ProcessExpanderState(Expander expander)
        {
            Grid parent = FindAncestor<Grid>(expander);
            int rowIndex = Grid.GetRow(expander);

            if (parent.RowDefinitions.Count > rowIndex && rowIndex >= 0)
                parent.RowDefinitions[rowIndex].Height =
                    (expander.IsExpanded ? new GridLength(1, GridUnitType.Star) : GridLength.Auto);
        }

        public static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            // Need this call to avoid returning current object if it is the 
            // same type as parent we are looking for
            current = VisualTreeHelper.GetParent(current);

            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            };
            return null;
        }

        private async void btnImportXml_Click(object sender, RoutedEventArgs e)
        {
            var allPaths = Configuration.GetSynergyV5XmlImportPaths();
            var count = 0;
            var total = allPaths.Count();

            await Task.Run(() =>
            {
                foreach (string path in allPaths)
                {
                    count++;

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        string fileName = System.IO.Path.GetFileName(path);

                        XDocument doc = Xml.Xml.DocumentFromPath(path);

                        List<SynergyV5List> allLists =
                            Xml.Xml.RetrieveSynergyV5Lists(doc);

                        foreach (SynergyV5List lst in allLists)
                        {
                            string detail = "path " + count + " of " + total;
                            detail += ": " + fileName + " -> ";
                            detail += "processing list: " + lst.ListName;

                            StatusDetailUpdate(detail);

                            db.Sync(lst);
                        }

                        File.Delete(path);
                    }

                }
            });

            statusDetail.Text = "finished.";
        }
        
        private void StatusDetailUpdate(string text)
        {
            //may need dispatcher async

            var currentTime = DateTime.Now;

            if ((DateTime.Now - previousTime).Milliseconds <= 50) return;

            syncContext.Post(new SendOrPostCallback(s =>
            {
                statusDetail.Text = (string)s;
            }), text);

            previousTime = currentTime;
        }

        private async void btnExportXml_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => {

                string detail;

                List<SynergyV5List> activeLists = new List<SynergyV5List>();

                var allListNames = db.GetAllActiveListNames();
                allListNames.AddRange(db.GetAllShelvedListNames());

                //mirrors Gauntlet 
                foreach (string listName in allListNames)
                {
                    SynergyV5List lst = new SynergyV5List(listName);

                    detail = "loading list: " + lst.ListName;

                    StatusDetailUpdate(detail);

                    //save() populates each list as part of its process
                    db.Sync(lst);

                    activeLists.Add(lst);
                }

                //XDocument doc =
                //    new XDocument(Xml.Xml.Export(activeLists));
                XElement synergySubsetEl = new XElement(Xml.Xml.TAG_SYNERGY_SUBSET);

                detail = "exporting lists to XML";

                StatusDetailUpdate(detail);

                foreach (SynergyV5List lst in activeLists)
                {

                    synergySubsetEl.Add(Xml.Xml.Export(lst));
                }

                XDocument doc =
                    new XDocument(
                        new XElement("nwd",
                            synergySubsetEl));

                //here, take doc and save to all sync locations            
                string fileName =
                    NwdUtils.GetTimeStamp_yyyyMMddHHmmss() + "-nwd-synergy-v5.xml";

                var allFolders =
                    Configuration.GetActiveSyncProfileIncomingXmlFolders();

                foreach (string xmlIncomingFolderPath in allFolders)
                {
                    string fullFilePath =
                        System.IO.Path.Combine(xmlIncomingFolderPath, fileName);

                    doc.Save(fullFilePath);
                }


            });

            statusDetail.Text = "finished.";
        }

        private void MenuItemShelveSelected_Click(object sender, RoutedEventArgs e)
        {
            IList items = (IList)lvSynergyV5ActiveLists.SelectedItems;
            var selectedItems = items.Cast<SynergyV5ListNode>();

            foreach (SynergyV5List sl in selectedItems.Select(x => x.List))
            {
                sl.Shelve();

                sl.Sync(db);
            }

            ExpandBothLists();
            
            Load();
        }

        private void MenuItemActivateSelected_Click(object sender, RoutedEventArgs e)
        {
            IList items = (IList)lvSynergyV5ShelvedLists.SelectedItems;
            var selectedItems = items.Cast<SynergyV5ListNode>();

            foreach (SynergyV5List sl in selectedItems.Select(x => x.List))
            {
                sl.Activate();

                sl.Sync(db);
            }

            ExpandBothLists();

            Load();
        }

        private void ExpandBothLists()
        {
            expActive.IsExpanded = true;
            expShelved.IsExpanded = true;
        }
    }

}
