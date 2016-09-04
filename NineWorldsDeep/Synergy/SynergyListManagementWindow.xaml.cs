using NineWorldsDeep.UI;
using System;
using System.Collections;
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
using System.Windows.Shapes;

namespace NineWorldsDeep.Synergy
{
    /// <summary>
    /// Interaction logic for SynergyListManagementWindow.xaml
    /// </summary>
    public partial class SynergyListManagementWindow : Window
    {
        private bool _isChanged = false;
        private ObservableCollection<SynergyList> _activeCol;
        private ObservableCollection<SynergyList> _inactiveCol;
        private Db.Sqlite.DbAdapterSwitch _sqliteDb =
            new Db.Sqlite.DbAdapterSwitch();

        public SynergyListManagementWindow()
        {
            InitializeComponent();
        }

        private void MoveAllToActive_Click(object sender, RoutedEventArgs e)
        {
            MoveAll(_inactiveCol, _activeCol);
        }

        private void MoveSelectedToActive_Click(object sender, RoutedEventArgs e)
        {
            MoveSelected(lvInactive, _inactiveCol, _activeCol);
        }

        private void MoveSelectedToInactive_Click(object sender, RoutedEventArgs e)
        {
            MoveSelected(lvActive, _activeCol, _inactiveCol);
        }

        private void MoveAllToInactive_Click(object sender, RoutedEventArgs e)
        {
            MoveAll(_activeCol, _inactiveCol);
        }

        private void MoveSelected(ListView from,
                                  ObservableCollection<SynergyList> fromCol,
                                  ObservableCollection<SynergyList> toCol)
        {            
            IList items = (IList)from.SelectedItems;
            var selectedLists = items.Cast<SynergyList>();

            List<SynergyList> toBeMoved = new List<SynergyList>();

            foreach (SynergyList selected in selectedLists)
            {
                toBeMoved.Add(selected);       
            }

            foreach(SynergyList selected in toBeMoved)
            {
                if (selected != null)
                {
                    fromCol.Remove(selected);
                    toCol.Add(selected);
                    IsChanged = true;
                }
            }
        }

        public bool IsChanged
        {
            get
            {
                return _isChanged;
            }

            private set
            {
                btnUpdateSqlite.IsEnabled = value;
                _isChanged = value;
            }
        }

        private void MoveAll(ObservableCollection<SynergyList> from,
                             ObservableCollection<SynergyList> to)
        {
            List<SynergyList> toBeMoved = new List<SynergyList>();

            foreach (SynergyList lst in from)
            {
                toBeMoved.Add(lst);
            }

            foreach (SynergyList lst in toBeMoved)
            {
                from.Remove(lst);
                to.Add(lst);
            }

            IsChanged = true;
        }

        private void RefreshMySql_Click(object sender, RoutedEventArgs e)
        {
            //LoadDbLists(_mySqlDb);
            Display.Message("not included in migration");
        }

        private void LoadDbLists(Db.Sqlite.DbAdapterSwitch db)
        {
            _activeCol = new ObservableCollection<SynergyList>(db.GetLists(true));
            _inactiveCol = new ObservableCollection<SynergyList>(db.GetLists(false));
            lvActive.ItemsSource = _activeCol;
            lvInactive.ItemsSource = _inactiveCol;

            IsChanged = false;
            tbDbName.Text = "SQLite";
        }

        private void UpdateMySql_Click(object sender, RoutedEventArgs e)
        {
            Display.Message("not included in migration");

            //UpdateDbActiveInactive(_mySqlDb, _activeCol, _inactiveCol);

            //foreach(ToDoList tdl in activeCol)
            //{
            //    mySqlDb.SetActive(tdl.Name, true);
            //}

            //foreach(ToDoList tdl in inactiveCol)
            //{
            //    mySqlDb.SetActive(tdl.Name, false);
            //}

            //LoadDbLists(mySqlDb);
        }

        //[Obsolete("use UpdateActiveInactive(db, lst, lst)")]
        //private void UpdateDb(IGauntletDbAdapter db)
        //{
        //    foreach (ToDoList tdl in _activeCol)
        //    {
        //        db.SetActive(tdl.Name, true);
        //    }

        //    foreach (ToDoList tdl in _inactiveCol)
        //    {
        //        db.SetActive(tdl.Name, false);
        //    }

        //    LoadDbLists(db);
        //}

        private void UpdateDbActiveInactive(Db.Sqlite.DbAdapterSwitch db, IEnumerable<SynergyList> setToActive, IEnumerable<SynergyList> setToInactive)
        {
            db.UpdateActiveInactive(setToActive, setToInactive);

            try
            {
                LoadDbLists(db);
            }
            catch (Exception ex)
            {
                Display.Exception(ex);
            }
        }

        private void RefreshSqlite_Click(object sender, RoutedEventArgs e)
        {
            LoadDbLists(_sqliteDb);
        }

        private void UpdateSqlite_Click(object sender, RoutedEventArgs e)
        {
            UpdateDbActiveInactive(_sqliteDb, _activeCol, _inactiveCol);
        }
    }
}
