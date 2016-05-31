using NineWorldsDeep.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace NineWorldsDeep.Synergy
{
    /// <summary>
    /// Interaction logic for SynergyV4MainWindow.xaml
    /// </summary>
    public partial class SynergyV4MainWindow : Window, INotifyPropertyChanged
    {
        private SynergyList _selectedList = null;
        private SyncHandler _sh = new SyncHandler();
        private SynergySqliteDbAdapter _db =
            new SynergySqliteDbAdapter();

        private ObservableCollection<SynergyList> _lists =
            new ObservableCollection<SynergyList>();

        public SynergyV4MainWindow()
        {
            InitializeComponent();
            _sh.DbSavePending = false;
            this.DataContext = this;
        }

        public IEnumerable<SynergyList> Lists
        {
            get
            {
                return _lists;
            }
        }

        public string NewList
        {
            set
            {
                if (SelectedList != null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(value))
                {
                    SynergyList lst = EnsureList(value);
                    SelectedList = lst;
                    lvLists.SelectedItem = lst;
                }
            }
        }

        public SynergyList SelectedList
        {
            get { return _selectedList; }
            set
            {
                _selectedList = value;
                OnPropertyChanged("SelectedList");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private SynergyList EnsureList(string listName)
        {
            SynergyList sl = GetListByName(listName);

            if (sl == null)
            {
                sl = new SynergyList()
                {
                    Name = listName
                };

                _lists.Add(sl);
            }

            return sl;
        }
        
        #region "File Menu Methods - TESTED"

        private void MenuItemLoadFromSqlite_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
                        
            Stopwatch sw = Stopwatch.StartNew();

            foreach(SynergyList sl in _db.GetActiveLists())
            {
                EnsureList(sl.Name).True(sl);
            }

            sw.Stop();
            string time = sw.Elapsed.ToString("mm\\:ss\\.fff");

            statusBar.StatusBarText = "Active Lists Loaded: " + time;
        }

        private void MenuItemSaveToSqlite_Click(object sender, RoutedEventArgs e)
        {          
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                _db.Save(Lists);
                sw.Stop();
            }
            catch(Exception ex)
            {
                Display.Exception(ex);
                sw.Stop();

                //in case it throws multiple errors(eg. an error for every row)
                return;
            }

            string time = sw.Elapsed.ToString("mm\\:ss\\.fff");
            statusBar.StatusBarText = "Current Lists Saved: " + time;
            _sh.DbSavePending = false;
        }

        private void MenuItemClearAll_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            _lists.Clear();
            SelectedList = null;
            txtInput.Text = "";
            ReLoadListItems(); //will clear list because selected list is null
            statusBar.StatusBarText = "All Lists Cleared.";
        }
        
        #endregion

        #region "Items Menu Methods - TESTED"

        private void MenuItemCompleteSelected_Click(object sender, RoutedEventArgs e)
        {
            SynergyItem si = (SynergyItem)lvItems.SelectedItem;
            if (si != null)
            {
                si.CompleteNow();
                ReLoadListItems();
            }
            else
            {
                statusBar.StatusBarText = "nothing selected";
            }
        }

        private void MenuItemArchiveSelected_Click(object sender, RoutedEventArgs e)
        {
            SynergyItem si = (SynergyItem)lvItems.SelectedItem;
            if (si != null)
            {
                si.ArchiveNow();
                ReLoadListItems();
            }
            else
            {
                statusBar.StatusBarText = "nothing selected";
            }
        }

        private void MenuItemUndoCompletionForSelected_Click(object sender, RoutedEventArgs e)
        {
            SynergyItem si = (SynergyItem)lvItems.SelectedItem;
            if (si != null)
            {
                si.UndoCompletion();
                ReLoadListItems();
            }
            else
            {
                statusBar.StatusBarText = "nothing selected";
            }
        }

        private void MenuItemUndoArchivalForSelected_Click(object sender, RoutedEventArgs e)
        {
            SynergyItem si = (SynergyItem)lvItems.SelectedItem;
            if (si != null)
            {
                si.UndoArchival();
                ReLoadListItems();
            }
            else
            {
                statusBar.StatusBarText = "nothing selected";
            }
        }

        private void MenuItemArchiveAllSelectedLists_Click(object sender, RoutedEventArgs e)
        {
            if(UI.Prompt.Confirm("Are you sure you want to archive ALL ITEMS in ALL SELECTED LISTS? This cannot be undone.", true)){

                foreach(var v in lvLists.SelectedItems)
                {
                    SynergyList lst = (SynergyList)v;

                    foreach(var si in lst.Items)
                    {
                        si.ArchiveNow();
                    }
                }
            }
        }

        #endregion

        #region "Sync Menu Methods - READY FOR TESTING"

        private void MenuItemExportActiveLists_Click(object sender, RoutedEventArgs e)
        {
            int ignoredCount = 0;

            _sh.ExportLists(ignoredCount, _db.GetActiveLists());

            if (ignoredCount > 0)
            {
                MessageBox.Show(ignoredCount +
                    " files already existed and were ignored. " +
                    "To export fresh versions of all lists, first" +
                    " consume all files then export again.");
            }

            statusBar.StatusBarText = "Files Exported.";
        }

        private void MenuItemSynergyListManagement_Click(object sender, RoutedEventArgs e)
        {
            new SynergyListManagementWindow().Show();
        }

        private void MenuItemImportSyncedFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _sh.ImportSynergyFiles();

                foreach (var sl in _sh.Lists)
                {
                    //TODO: needs testing
                    //make sure categorized items make it
                    //back into their source lists

                    var catItems = 
                        new Dictionary<SynergyItem, string>();
                    var toBeRemoved =
                        new List<SynergyItem>();

                    foreach(var si in sl.Items)
                    {
                        if (GauntletUtils.IsCategorizedItem(si.Item))
                        {
                            string cat = 
                                GauntletUtils.ParseCategory(si.Item);
                            string trimmedItem = 
                                GauntletUtils.TrimCategory(si.Item);

                            SynergyItem newItem =
                                new SynergyItem();
                            newItem.Item = trimmedItem;
                            newItem.True(si);

                            catItems.Add(newItem, cat);
                            toBeRemoved.Add(si);
                        }
                    }

                    foreach(SynergyItem si in catItems.Keys)
                    {
                        EnsureList(catItems[si]).AddItem(si);
                    }

                    foreach(SynergyItem si in toBeRemoved)
                    {
                        sl.RemoveItem(si);
                    }

                    EnsureList(sl.Name).True(sl);
                }
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }
        }

        private void MenuItemImportSyncedArchiveFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _sh.ImportSyncedArchiveFiles();

                foreach (var sl in _sh.Lists)
                {
                    EnsureList(sl.Name).True(sl);
                }
                
                statusBar.StatusBarText = "Synergy Archive Files Imported.";
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }
        }

        private void MenuItemConsumeAllImportedFiles_Click(object sender, RoutedEventArgs e)
        {
            if (_sh.DbSavePending)
            {
                MessageBox.Show("Sqlite Db Save Pending, Consume Aborted.");
            }
            else
            {
                //TODO:  use Display.Grid(filesToConsume) but verify that we can receive a boolean (add OK/Cancel to Display.Grid()?), for now, just displaying and then prompting
                Display.Grid("files to consume", _sh.FilesToConsume.ToListItems());

                string msg = "The following files will be consumed, proceed?";

                foreach (string path in _sh.FilesToConsume)
                {
                    msg += Environment.NewLine + path;
                }

                if (NineWorldsDeep.UI.Prompt.Confirm(msg, true))
                {
                    int count = _sh.ConsumeFiles();

                    statusBar.StatusBarText = count + " files consumed.";
                }
                else
                {
                    statusBar.StatusBarText = "Consume Aborted.";
                }
            }
        }

        #endregion

        #region "Event Methods - TESTED"

        private void txtListName_KeyUp(object sender, KeyEventArgs e)
        {
            //TODO: we can add predictive text without losing our current bevior, 
            //but it will take a bit more time than I have at the moment
            //heres how I'm thinking: as we type (second block below)
            //instead of GetListByName() we call GetFirstListThatStartsWith(listNamePartial)
            //which still returns null if nothing is found, but otherwise returns
            //the first list that starts with the current value
            //then we populate the textbox with that list's name, but
            //we set selection on all letters that are not included in the original entry
            //(eg. if we type "Tes" and there is a list called "Testing" it would
            //set txtListName to "Test" with the final "t" highlighted, so if
            //a user keeps typing, it is overwritten with the next letter and the 
            //process starts all over again to match another list if it exists
            //
            //NB: if enter is pressed, make sure it doesn't erase the highlighted text
            //
            //NB: re: listNameExists/StartsWith(): for any camel cased letters in the input, 
            //insert spaces prior to processing. Spaces are the expected separator, but 
            //if type ahead is used, the list names populating it will be PascalCase. A 
            //conversion is one way to ensure uniformity of behavior, for both input cases.



            if (e.Key == Key.Enter)
            {
                txtListName.Text = SynergyUtils.ProcessListName(txtListName.Text);
                BindingExpression exp = this.txtListName.GetBindingExpression(TextBox.TextProperty);
                exp.UpdateSource();
                txtListName.Text = "";
            }
            else
            {
                //check as we type
                string listName = SynergyUtils.ProcessListName(txtListName.Text);

                SynergyList sl = GetListByName(listName);

                if (sl != null)
                {
                    SelectedList = sl;
                }
                else
                {
                    SelectedList = null;
                }
            }
        }
        
        /// <summary>
        /// returns null if not found
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public SynergyList GetListByName(string listName)
        {
            SynergyList sl = null;

            foreach (SynergyList lst in Lists)
            {
                if (lst.Name.Equals(listName))
                {
                    sl = lst;
                }
            }

            return sl;
        }

        private void lvLists_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ReLoadListItems();
        }

        private void ReLoadListItems()
        {
            lvItems.ItemsSource = null;

            SynergyList lst = (SynergyList)lvLists.SelectedItem;
            if (lst != null)
            {
                lvItems.ItemsSource = lst.Items;
            }
        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddEnteredItem();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddEnteredItem();
        }

        private void AddEnteredItem()
        {
            SynergyList lst = (SynergyList)lvLists.SelectedItem;
            if (lst != null)
            {
                string item = txtInput.Text.Trim();
                if (!string.IsNullOrWhiteSpace(item))
                {
                    //lst.AddWithMerge(new SynergyItem() { Description = item, Completed = false });
                    lst.AddItem(new SynergyItem()
                    {
                        Item = item
                    });
                    txtInput.Text = "";
                    ReLoadListItems();
                }
            }
        }

        #endregion

    }
}
