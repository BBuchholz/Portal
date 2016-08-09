using NineWorldsDeep.Core;
using NineWorldsDeep.UI;
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
using System.Windows.Shapes;

namespace NineWorldsDeep.Warehouse
{
    /// <summary>
    /// Interaction logic for SyncMapWindow.xaml
    /// </summary>
    public partial class SyncMapWindow : Window
    {
        private SyncMapCollection _syncMapCol;
        private SyncProfile _syncProfile;

        private Db.SqliteDbAdapter db =
            new Db.SqliteDbAdapter();

        public SyncMapWindow(SyncProfile syncProfile)
        {
            InitializeComponent();
            _syncProfile = syncProfile;

            //SetDemoProfile();
            //LoadProfileFromDb();
            RefreshViewFromProfile(true);
        }

        private void LoadProfileFromDb()
        {
            Display.Message(db.LoadSyncProfile(_syncProfile));
        }

        private void RefreshViewFromProfile()
        {
            RefreshViewFromProfile(false);
        }

        private void RefreshViewFromProfile(bool reloadFromDbFirst)
        {
            SyncMaps.Clear();

            if (reloadFromDbFirst)
            {
                LoadProfileFromDb();
            }

            if (_syncProfile != null)
            {
                tbProfileName.Text = _syncProfile.Name;

                foreach (SyncMap sm in _syncProfile.SyncMaps)
                {
                    SyncMaps.Add(sm);
                }
            }
            else
            {
                tbProfileName.Text = "no profile";
            }

        }

        private void SetDemoProfile()
        {
            _syncProfile = new SyncProfile("demo");

            _syncProfile.SyncMaps.Add(
                new SyncMap(_syncProfile,
                            SyncDirection.Import,
                            SyncAction.Ignore)
                {
                    Source = @"C:\Source\Path\Here",
                    Destination = @"C:\Destination\Path\Here"
                });

            _syncProfile.SyncMaps.Add(
                new SyncMap(_syncProfile,
                            SyncDirection.Export,
                            SyncAction.Ignore)
                {
                    Source = @"C:\Source\Path\Here",
                    Destination = @"C:\Destination\Path\Here"
                });

            _syncProfile.SyncMaps.Add(
                new SyncMap(_syncProfile,
                            SyncDirection.Import,
                            SyncAction.Ignore)
                {
                    Source = @"C:\Source\Path\Here",
                    Destination = @"C:\Destination\Path\Here"
                });

            _syncProfile.SyncMaps.Add(
                new SyncMap(_syncProfile,
                            SyncDirection.Export,
                            SyncAction.Ignore)
                {
                    Source = @"C:\Source\Path\Here",
                    Destination = @"C:\Destination\Path\Here"
                });
        }

        /// <summary>
        /// retrieves XAML defined collection with lazy instantiation
        /// </summary>
        private SyncMapCollection SyncMaps
        {
            get
            {
                if (_syncMapCol == null)
                {
                    _syncMapCol = (SyncMapCollection)
                        FindResource("colSyncMaps");
                }

                return _syncMapCol;
            }
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {

        }

        private void MenuItemAddImportSyncMap_Click(object sender, RoutedEventArgs e)
        {
            string defaultPath = Configuration.SyncRoot(_syncProfile.Name);
            string fromPath = UI.Prompt.ForFolder(defaultPath, "Select 'Import From' Folder");

            if (fromPath != null)
            {
                string toPath = UI.Prompt.ForFolder(Configuration.NwdAuxFolder, "Select 'Import To' Folder");

                if (toPath != null)
                {
                    _syncProfile.SyncMaps.Add(
                        new SyncMap(_syncProfile,
                                    SyncDirection.Import,
                                    SyncAction.Ignore)
                        {
                            Source = fromPath,
                            Destination = toPath
                        });

                    RefreshViewFromProfile();
                }
            }

        }

        private void MenuItemAddExportSyncMap_Click(object sender, RoutedEventArgs e)
        {
            string defaultPath = Configuration.SyncRoot(_syncProfile.Name);
            string fromPath = UI.Prompt.ForFolder(defaultPath, "Select 'Export From' Folder");

            if (fromPath != null)
            {
                string toPath = UI.Prompt.ForFolder(Configuration.NwdAuxFolder, "Select 'Export To' Folder");

                if (toPath != null)
                {
                    _syncProfile.SyncMaps.Add(
                        new SyncMap(_syncProfile,
                                    SyncDirection.Export,
                                    SyncAction.Ignore)
                        {
                            Source = fromPath,
                            Destination = toPath
                        });

                    RefreshViewFromProfile();
                }
            }
        }

        private void MenuItemSaveToSqlite_Click(object sender, RoutedEventArgs e)
        {
            Display.Message(db.SaveSyncProfile(_syncProfile));
        }

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            foreach (DataGridColumn col in dataGrid.Columns)
            {
                //only SyncAction should be modifiable, everything else should be process populated
                if (!col.Header.Equals("DefaultSyncAction"))
                {
                    col.IsReadOnly = true;
                }
            }
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.Equals("DefaultSyncAction"))
            {
                SyncAction newAction = (SyncAction)(e.EditingElement as ComboBox).SelectedItem;
                //Display.Message(action.ToString());
                SyncMap map = (SyncMap)e.Row.Item;

                foreach (SyncMap m in _syncMapCol)
                {
                    if (map.Destination.Equals(m.Destination) &&
                        map.Source.Equals(m.Source) &&
                        map.SyncDirection.Equals(m.SyncDirection))
                    {
                        m.DefaultSyncAction = newAction;
                    }
                }
            }
        }

        private void MenuItemDeleteSyncMap_Click(object sender, RoutedEventArgs e)
        {
            SyncMap sm = (SyncMap)dataGrid.SelectedItem;

            if (sm != null && UI.Prompt.Confirm("Are you sure you want to delete the selected SyncMap?", true))
            {
                Display.Message(db.DeleteSyncMap(sm));

                //LoadProfileFromDb();
                RefreshViewFromProfile(true);
            }
        }
    }
}
