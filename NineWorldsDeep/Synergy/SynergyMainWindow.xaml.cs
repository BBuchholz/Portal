using NineWorldsDeep.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for SynergyMainWindow.xaml
    /// </summary>
    public partial class SynergyMainWindow : Window, INotifyPropertyChanged, IListMatrix
    {

        private ToDoList _selectedList;

        //TODO: wrap the db and the file based storage strategies into strategy pattern classes
        //private string toDoListsFile = "C:\\NWD\\synergy\\categorizedLists.xml";

        private ObservableCollection<ToDoList> _lists =
            new ObservableCollection<ToDoList>();

        private MenuController _menu;
        private GauntletMenuController _gauntletMenuController;

        public SynergyMainWindow()
        {
            try
            {
                InitializeComponent();
                this.DataContext = this;

                string deprecationMessage =
                    "This version of NwdSynergy is slated for retirement " +
                    "5/21/2016, when db tables related to the previous version " +
                    "will be dropped. You should be using SynergyV4 instead.";

                //NB: when this goes, drop table junction_List_Item_Status (after searching for any remaining usages, of course)

                Display.Message(deprecationMessage);

                _menu = new MenuController();
                _menu.Configure(mainMenu);

                //IGauntletDbAdapter mysqlDb = new GauntletModelMySqlDbAdapter(
                //    ConnectionStrings.ByName("MySqlConnectionString"));
                IGauntletDbAdapter sqliteDb = new GauntletModelSqliteDbAdapter();

                _gauntletMenuController = new GauntletMenuController();
                _gauntletMenuController.Configure(_menu, this, sqliteDb, statusBar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public IEnumerable<ToDoList> Lists
        {
            get
            {
                return _lists;
            }
        }

        public ToDoList SelectedList
        {
            get { return _selectedList; }
            set
            {
                _selectedList = value;
                //OnPropertyChanged("SelectedItem");
                OnPropertyChanged("SelectedList");
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
                    ToDoList lst = EnsureList(value);
                    SelectedList = lst;
                    //cmbLists.SelectedItem = lst;
                    lvLists.SelectedItem = lst;
                }
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

        //private void cmbLists_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        BindingExpression exp = this.cmbLists.GetBindingExpression(ComboBox.TextProperty);
        //        exp.UpdateSource();
        //    }
        //}

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

                ToDoList tdi = GetListByName(listName);

                if(tdi != null)
                {
                    SelectedList = tdi;
                }
                else
                {
                    SelectedList = null;
                }
            }
        }

        //private void cmbLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ReLoadListItems();
        //}

        private void lvLists_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ReLoadListItems();
        }

        private void ReLoadListItems()
        {
            lvItems.ItemsSource = null;

            //ToDoList lst = (ToDoList)cmbLists.SelectedItem;
            ToDoList lst = (ToDoList)lvLists.SelectedItem;
            if (lst != null)
            {
                lvItems.ItemsSource = lst.Items;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddEnteredItem();
        }

        private void AddEnteredItem()
        {
            //ToDoList lst = (ToDoList)cmbLists.SelectedItem;
            ToDoList lst = (ToDoList)lvLists.SelectedItem;
            if (lst != null)
            {
                string item = tbInput.Text.Trim();
                if (!string.IsNullOrWhiteSpace(item))
                {
                    lst.AddWithMerge(new ToDoItem() { Description = item, Completed = false });
                    tbInput.Text = "";
                    ReLoadListItems();
                }
            }
        }

        private void tbInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddEnteredItem();
            }
        }

        private void MenuItemLoadFromMySql_Click(object sender, RoutedEventArgs e)
        {
            Display.Message("mysql support not included in migration");

            //_lists.Clear();
            //statusBar.StatusBarText = db.LoadFromDb(this);
            //_gauntletMenuController.LoadFromMySql();
            //statusBar.StatusBarText += Environment.NewLine +
            //    new ListMatrixXmlAdapter(this).LoadFromFile(toDoListsFile);
        }

        /// <summary>
        /// returns null if not found
        /// </summary>
        /// <param name="listName"></param>
        /// <returns></returns>
        public ToDoList GetListByName(string listName)
        {
            ToDoList tdi = null;

            foreach (ToDoList lst in Lists)
            {
                if (lst.Name.Equals(listName))
                {
                    tdi = lst;
                }
            }

            return tdi;
        }

        public ToDoList EnsureList(string listName)
        {
            //ToDoList tdi = null;

            //foreach (ToDoList lst in Lists)
            //{
            //    if (lst.Name.Equals(listName))
            //    {
            //        tdi = lst;
            //    }
            //}

            ToDoList tdi = GetListByName(listName);

            if (tdi == null)
            {
                tdi = new ToDoList() { Name = listName };
                _lists.Add(tdi);
            }

            return tdi;
        }

        private void MenuItemSaveToMySql_Click(object sender, RoutedEventArgs e)
        {
            //statusBar.StatusBarText = 
            //    new ListMatrixXmlAdapter(this).SaveToFile(toDoListsFile);
            //statusBar.StatusBarText += Environment.NewLine + db.SaveToDb();

            //statusBar.StatusBarText = db.SaveToDb(this);
            _gauntletMenuController.SaveToMySql();
        }

        private void MenuItemSaveToSqlite_Click(object sender, RoutedEventArgs e)
        {
            _gauntletMenuController.SaveToSqlite();
        }

        private void MenuItemLoadFromSqlite_Click(object sender, RoutedEventArgs e)
        {
            _lists.Clear();
            try
            {
                _gauntletMenuController.LoadFromSqlite();
            }
            catch(Exception ex)
            {
                Display.Exception(ex);
            }
        }

        private void MenuItemCompleteSelected_Click(object sender, RoutedEventArgs e)
        {
            ToDoItem tdi = (ToDoItem)lvItems.SelectedItem;
            if (tdi != null)
            {
                tdi.Completed = true;
                ReLoadListItems();
            }
            else
            {
                statusBar.StatusBarText = "nothing selected";
            }
        }

        private void MenuItemDbTestHarness_Click(object sender, RoutedEventArgs e)
        {
            Display.Message("left out of migration");
        }

        private void MenuItemClearAll_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        public void ClearAll()
        {
            _lists.Clear();
            SelectedList = null;
            ReLoadListItems(); //will clear list because selected list is null
            statusBar.StatusBarText = "All Lists Cleared.";
        }

        private void MenuItemLoadAllFromMySql_Click(object sender, RoutedEventArgs e)
        {
            _lists.Clear();
            _gauntletMenuController.LoadFromMySql(true);
        }
        
    }
}
