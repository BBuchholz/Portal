using NineWorldsDeep.UI;
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
using System.Windows.Shapes;

namespace NineWorldsDeep.Synergy
{
    /// <summary>
    /// Interaction logic for ActiveListManagementWindow.xaml
    /// </summary>
    public partial class ActiveListManagementWindow : Window
    {
        private bool _isChanged = false;
        //private IGauntletDbAdapter _mySqlDb;
        private IGauntletDbAdapter _sqliteDb;
        private ObservableCollection<ToDoList> _activeCol;
        private ObservableCollection<ToDoList> _inactiveCol;

        public ActiveListManagementWindow(IGauntletDbAdapter sqliteDb)
        {
            InitializeComponent();
            //this._mySqlDb = mySqlDb;
            this._sqliteDb = sqliteDb;
            //LoadDbLists(mySqlDb);
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
                                  ObservableCollection<ToDoList> fromCol,
                                  ObservableCollection<ToDoList> toCol)
        {
            List<ToDoList> selectedLists = (List<ToDoList>)from.SelectedItems;
            
            foreach(ToDoList selected in selectedLists)
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
                btnUpdateMySql.IsEnabled = value;
                btnUpdateSqlite.IsEnabled = value;
                _isChanged = value;
            }
        }

        private void MoveAll(ObservableCollection<ToDoList> from,
                             ObservableCollection<ToDoList> to)
        {
            List<ToDoList> toBeMoved = new List<ToDoList>();

            foreach (ToDoList lst in from)
            {
                toBeMoved.Add(lst);
            }

            foreach (ToDoList lst in toBeMoved)
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

        private void LoadDbLists(IGauntletDbAdapter db)
        {
            _activeCol = new ObservableCollection<ToDoList>(db.GetLists(true));
            _inactiveCol = new ObservableCollection<ToDoList>(db.GetLists(false));
            lvActive.ItemsSource = _activeCol;
            lvInactive.ItemsSource = _inactiveCol;

            IsChanged = false;
            tbDbName.Text = GetDbName(db);
        }

        private string GetDbName(IGauntletDbAdapter db)
        {
            string name = "[unknown db]";

            //if (db.Equals(_mySqlDb))
            //{
            //    return "MySQL";
            //}

            if (db.Equals(_sqliteDb))
            {
                return "SQLite";
            }

            return name;
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

        [Obsolete("use UpdateActiveInactive(db, lst, lst)")]
        private void UpdateDb(IGauntletDbAdapter db)
        {
            foreach (ToDoList tdl in _activeCol)
            {
                db.SetActive(tdl.Name, true);
            }

            foreach (ToDoList tdl in _inactiveCol)
            {
                db.SetActive(tdl.Name, false);
            }

            LoadDbLists(db);
        }

        private void UpdateDbActiveInactive(IGauntletDbAdapter db, IEnumerable<ToDoList> setToActive, IEnumerable<ToDoList> setToInactive)
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
