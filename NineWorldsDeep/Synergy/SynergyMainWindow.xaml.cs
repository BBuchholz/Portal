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
                    cmbLists.SelectedItem = lst;
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

        private void cmbLists_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression exp = this.cmbLists.GetBindingExpression(ComboBox.TextProperty);
                exp.UpdateSource();
            }
        }

        private void cmbLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReLoadListItems();
        }

        private void ReLoadListItems()
        {
            lvItems.ItemsSource = null;

            ToDoList lst = (ToDoList)cmbLists.SelectedItem;
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
            ToDoList lst = (ToDoList)cmbLists.SelectedItem;
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

        public ToDoList EnsureList(string listName)
        {
            ToDoList tdi = null;

            foreach (ToDoList lst in Lists)
            {
                if (lst.Name.Equals(listName))
                {
                    tdi = lst;
                }
            }

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
